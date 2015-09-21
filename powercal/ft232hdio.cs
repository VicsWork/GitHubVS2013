using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using FTD2XX_NET;

namespace DIO
{
    public class FT232HDIO
    {
        FTDI _ftdi = new FTDI();

        /// <summary>
        /// Track the state of the output pins
        /// </summary>
        byte _cha_state = 0x00;
        byte _chb_state = 0x00;

        /// <summary>
        /// AD_BUS = D[0:7], AC_BUS = C[0:7]
        /// </summary>
        public enum DIO_BUS { AD_BUS, AC_BUS };
        /// <summary>
        /// The pin numbers
        /// </summary>
        public enum PIN { PIN0 = 0, PIN1 = 1, PIN2 = 2, PIN3 = 3, PIN4 = 4, PIN5 = 5, PIN6 = 6, PIN7 = 7 };

        FTD2XX_NET.FTDI.FT_DEVICE _dev_type;
        public FTD2XX_NET.FTDI.FT_DEVICE DeviceType { get { return _dev_type; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <see cref="http://www.ftdichip.com/Support/Documents/AppNotes/AN_108_Command_Processor_for_MPSSE_and_MCU_Host_Bus_Emulation_Modes.pdf"/>
        /// <param name="dev_index"></param>
        /// <returns></returns>
        public FTDI.FT_STATUS ResetDevice()
        {
            FTDI.FT_STATUS status = _ftdi.ResetDevice();
            return status;
        }


        /// <summary>
        /// Open the FTDI device
        /// </summary>
        /// <param name="index"></param>
        public void Open(uint index)
        {
            //_ftdi.Close();
            FTDI.FT_STATUS status = _ftdi.OpenByIndex(index);
            if (status != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception( string.Format("Problem opening FTDI device at index {0}", index) );
            }

            //// Enable MPSSE
            //status = _ftdi.SetBitMode(0xFF, FTDI.FT_BIT_MODES.FT_BIT_MODE_MPSSE);
            //if (status != FTDI.FT_STATUS.FT_OK)
            //{
            //    throw new Exception(string.Format("Problem setting FTDI bitmode for device at index {0}", index));
            //}
            //_ftdi.SetBitMode(0, 0);
            //_ftdi.SetBitMode(0, 2);

        }

        public FTDI.FT_STATUS SetModeMPSSE()
        {
            // Enable MPSSE
            FTDI.FT_STATUS status = _ftdi.SetBitMode(0xFF, FTDI.FT_BIT_MODES.FT_BIT_MODE_MPSSE);
            if (status != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception(string.Format("Problem setting FTDI bitmode"));
            }

            return status;
        }

        /// <summary>
        /// Returns the index of the first available FTDI 232H device found in the system
        /// </summary>
        /// <returns></returns>
        public int GetFirstDevIndex()
        {
            int count = 10;

            FTDI.FT_DEVICE_INFO_NODE[] devlist = new FTDI.FT_DEVICE_INFO_NODE[count];
            FTDI.FT_STATUS status = _ftdi.GetDeviceList(devlist);
            Debug.Assert(status == FTDI.FT_STATUS.FT_OK, "Problem getting FTDI device list");

            int index = -1;
            for (int i = 0; i < count; i++)
            {
                FTDI.FT_DEVICE_INFO_NODE devinfo = devlist[i];
                if (devinfo != null)
                {
                    if (devinfo.Type == FTD2XX_NET.FTDI.FT_DEVICE.FT_DEVICE_232H)
                    {
                        index = i;
                        _dev_type = devinfo.Type;
                        break;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Sets directions of the pins in a bus
        /// </summary>
        /// <param name="bus"></param>
        void SetPortAsOutput(DIO_BUS bus, byte direction=0xFF)
        {
            // Set all outputs and data = all 0
            //(0x80, level_low, dir_low, 0x82, level_high, dir_high)
            //byte[] data = new byte[] { 0x80, _cha_state, 0xFF, 0x82, _chb_state, 0xFF };

            byte addr = get_bus_address(bus);
            byte state = get_bus_state(bus);

            byte[] data = new byte[] { addr, state, direction };
            uint n = 0;
            FTDI.FT_STATUS status = _ftdi.Write(data, data.Length, ref n);
            if (status != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception(string.Format("Problem writing to bus {0}", bus.ToString()));
            }

            set_bus_state(bus, state);

        }

        /// <summary>
        /// Gets the address for each data bus
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        byte get_bus_address(DIO_BUS bus)
        {
            byte addr = 0x80;
            if (bus == DIO_BUS.AC_BUS)
                addr = 0x82;
            return addr;
        }

        /// <summary>
        /// Gets the state of a the pins for a particular bus
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        byte get_bus_state(DIO_BUS bus)
        {
            byte state = _cha_state;
            if (bus == DIO_BUS.AC_BUS)
                state = _chb_state;
            return state;
        }

        /// <summary>
        /// Set the bus pins
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        FTDI.FT_STATUS set_bus_state(DIO_BUS bus, byte state)
        {
            if (bus == DIO_BUS.AD_BUS)
                _cha_state = state;
            else if (bus == DIO_BUS.AC_BUS)
                _chb_state = state;

            byte addr = get_bus_address(bus);
            byte[] buffer = new byte[] { addr, state, 0xFF };
            uint n = 0;
            FTDI.FT_STATUS status = _ftdi.Write(buffer, buffer.Length, ref n);

            return status;

        }

        /// <summary>
        /// Set the state of the pin
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="pin"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public FTDI.FT_STATUS SetPin(DIO_BUS bus, uint pin, bool value)
        {
            Debug.Assert(pin < 8 && pin >= 0, "Pin number must be between 0 and 7");

            byte pin_num = Convert.ToByte(pin);
            byte state_current = get_bus_state(bus);
            byte state_new = state_current;
            if (value)
            {
                state_new |= (byte)(1 << pin_num);
            }
            else
            {
                state_new &= (byte)(~(1 << pin_num) & 0xFF);
            }

            FTDI.FT_STATUS status = set_bus_state(bus, state_new);
            return status;
        }

        /// <summary>
        /// Set the state of the pin
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="pin"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public FTDI.FT_STATUS SetPin(DIO_BUS bus, PIN pin, bool value)
        {
            byte pin_num = Convert.ToByte(pin);
            FTDI.FT_STATUS status = SetPin(bus, pin_num, value);
            return status;
        }

        /// <summary>
        /// Reset the port
        /// </summary>
        /// <returns></returns>
        public FTDI.FT_STATUS ResetPort()
        {
            FTDI.FT_STATUS status = _ftdi.ResetPort();
            status = _ftdi.Purge(FTDI.FT_PURGE.FT_PURGE_RX | FTDI.FT_PURGE.FT_PURGE_TX);
            return status;
        }

        /// <summary>
        /// Close the port
        /// </summary>
        /// <returns></returns>
        public FTDI.FT_STATUS Close()
        {
            FTDI.FT_STATUS status = _ftdi.Close();
            return status;
        }

        /// <summary>
        /// Close the port
        /// </summary>
        /// <returns></returns>
        public FTDI.FT_STATUS Rescan()
        {
            FTDI.FT_STATUS status = _ftdi.Rescan();
            return status;
        }


    }
}
