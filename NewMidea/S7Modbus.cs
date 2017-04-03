using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO.Ports;
namespace NewMideaProgram
{
    public class S7Modbus
    {
        /// <summary>
        /// 接收数据超时时间 单位ms
        /// </summary>
        public int TimeOut
        { get; set; }
        /// <summary>
        /// 发送数据串口
        /// </summary>
        public SerialPort Com
        { get; set; }
        /// <summary>
        /// PLC地址
        /// </summary>
        public byte Address
        { get; set; }
        /// <summary>
        /// 错误
        /// </summary>
        public string Error
        { get; set; }
        public S7Modbus(SerialPort com, byte address)
        {
            this.Com = com;
            this.TimeOut = 500;
            this.Address = address;
            this.Error = "";
        }
        public bool WriteVB(int start, byte[] value)
        {
            ushort tmp = 0;
            short[] tmpBuff = new short[value.Length / 2];
            if ((value.Length % 2) == 0)
            {
                tmpBuff = new short[value.Length / 2];
                for (int i = 0; i < tmpBuff.Length; i++)
                {
                    tmp = (ushort)(value[i * 2] * 0x100 + value[i * 2 + 1]);
                    if (tmp >= 0x8000)
                    {
                        tmpBuff[i] = (short)(-((tmp ^ 0xFFFF) + 1));
                    }
                    else
                    {
                        tmpBuff[i] = (short)(tmp);
                    }
                }
                return WriteVW(start, tmpBuff);
            }
            else
            {
                byte[] tmpByte;

                tmpBuff = new short[(value.Length + 1) / 2];


                if ((start % 2) == 0)
                {
                    if (!ReadVB(start + value.Length, 1, out tmpByte))
                    {
                        return false;
                    }
                    for (int i = 0; i < tmpBuff.Length - 1; i++)
                    {
                        tmp = (ushort)(value[i * 2] * 0x100 + value[i * 2 + 1]);
                        if (tmp >= 0x8000)
                        {
                            tmpBuff[i] = (short)(-((tmp ^ 0xFFFF) + 1));
                        }
                        else
                        {
                            tmpBuff[i] = (short)(tmp);
                        }
                    }
                    tmp = (ushort)(value[tmpBuff.Length * 2 - 2] * 0x100 + tmpByte[0]);
                    if (tmp >= 0x8000)
                    {
                        tmpBuff[tmpBuff.Length - 1] = (short)(-((tmp ^ 0xFFFF) + 1));
                    }
                    else
                    {
                        tmpBuff[tmpBuff.Length - 1] = (short)(tmp);
                    }
                    return WriteVW(start, tmpBuff);
                }
                else
                {
                    if (!ReadVB(start - 1, 1, out tmpByte))
                    {
                        return false;
                    }
                    tmp = (ushort)(tmpByte[0] * 0x100 + value[0]);
                    if (tmp >= 0x8000)
                    {
                        tmpBuff[0] = (short)(-((tmp ^ 0xFFFF) + 1));
                    }
                    else
                    {
                        tmpBuff[0] = (short)(tmp);
                    }
                    for (int i = 1; i < tmpBuff.Length; i++)
                    {
                        tmp = (ushort)(value[i * 2 - 1] * 0x100 + value[i * 2]);
                        if (tmp >= 0x8000)
                        {
                            tmpBuff[i] = (short)(-((tmp ^ 0xFFFF) + 1));
                        }
                        else
                        {
                            tmpBuff[i] = (short)tmp;
                        }
                    }
                    return WriteVW(start - 1, tmpBuff);
                }
            }
        }

        private byte[] Short2Byte(short[] value)
        {
            byte[] result = new byte[value.Length * 2];
            ushort tmp = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] < 0)
                {
                    tmp = (ushort)(-((value[i] ^ 0xFFFF) + 1));
                }
                else
                {
                    tmp = (ushort)value[i];
                }
                result[i * 2] = (byte)((tmp >> 8) & 0xFF);
                result[i * 2 + 1] = (byte)(tmp & 0xFF);
            }
            return result;
        }
        public bool WriteVW(int start, short[] value)
        {
            bool result = true;
            byte[] sendTmpBuff;
            byte[] tmpByte;
            try
            {
                int curStart = (int)(Math.Floor(start / 2.0f));
                if ((start % 2) == 1)
                {
                    sendTmpBuff = new byte[value.Length * 2 + 2];
                    tmpByte = Short2Byte(value);
                    if (!ReadVB(start - 1, value.Length * 2 + 2, out sendTmpBuff))
                    {
                        return false;
                    }
                    for (int i = 0; i < value.Length * 2; i++)
                    {
                        sendTmpBuff[i + 1] = tmpByte[i];
                    }
                }
                else
                {
                    sendTmpBuff = Short2Byte(value);
                }


                byte[] sendBuff = new byte[sendTmpBuff.Length + 9];
                sendBuff[0] = Address;
                sendBuff[1] = 0x10;
                sendBuff[2] = (byte)(curStart >> 8);
                sendBuff[3] = (byte)(curStart & 0xFF);
                sendBuff[4] = (byte)((sendTmpBuff.Length / 2) >> 8);
                sendBuff[5] = (byte)((sendTmpBuff.Length / 2) & 0xFF);
                sendBuff[6] = (byte)((sendTmpBuff.Length / 2) * 2);
                for (int i = 0; i < sendTmpBuff.Length; i++)
                {
                    sendBuff[7 + i] = sendTmpBuff[i];
                }
                Crc16(sendBuff, 7 + sendTmpBuff.Length, out sendBuff[7 + sendTmpBuff.Length], out  sendBuff[8 + sendTmpBuff.Length]);

                bool timeOut = false;
                bool getData = false;
                int timeOuts = 0;
                if (!Com.IsOpen)
                {
                    Com.Open();
                }
                Com.Write(sendBuff, 0, sendBuff.Length);
                do
                {
                    Thread.Sleep(50);
                    if (Com.BytesToRead >= 8)
                    {
                        getData = true;
                    }
                    if ((timeOuts * 50) >= TimeOut)
                    {
                        timeOut = true;
                    }
                    timeOuts++;
                }
                while (!timeOut && !getData);
                if (timeOut && !getData)
                {
                    Error = "写入数据超时,请检查连接";
                    result = false;
                }
                else
                {
                    byte[] readBuff = new byte[Com.BytesToRead];
                    Com.Read(readBuff, 0, readBuff.Length);
                    if (readBuff[0] != sendBuff[0] || sendBuff[1] != readBuff[1])
                    {
                        Error = "写入数据校验错误";
                        result = false;
                    }
                }
            }
            catch (Exception e)
            {
                Error = e.Message;
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 将数组以字符串的形式输出
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Hex2Str(byte[] buff, int start, int len)
        {
            string result = "";
            for (int i = start; i < buff.Length && i < start + len; i++)
            {
                result = string.Format("{0}{1:X2}", result, buff[i]);
            }
            return result;
        }
        public bool ReadVB(int start, int len, out byte[] value)
        {
            bool result = true;
            value = new byte[len];
            int curLen = (int)(Math.Floor(len / 2.0f)) + 2;
            int curIndex = 0;
            int curStart = (int)(Math.Floor(start / 2.0f));
            if ((start % 2) == 1)
            {
                curIndex = 1;
            }
            try
            {
                byte[] sendBuff = new byte[8];
                sendBuff[0] = Address;
                sendBuff[1] = 0x03;
                sendBuff[2] = (byte)(curStart >> 8);
                sendBuff[3] = (byte)(curStart & 0xFF);
                sendBuff[4] = (byte)(curLen >> 8);
                sendBuff[5] = (byte)(curLen & 0xFF);


                Crc16(sendBuff, 6, out sendBuff[6], out sendBuff[7]);

                //System.Windows.Forms.MessageBox.Show(Hex2Str(sendBuff, 0, sendBuff.Length));

                bool timeOut = false;
                bool getData = false;
                int timeOuts = 0;
                if (!Com.IsOpen)
                {
                    Com.Open();
                }
                Com.Write(sendBuff, 0, sendBuff.Length);
                do
                {
                    Thread.Sleep(50);
                    if (Com.BytesToRead >= curLen * 2 + 5)
                    {
                        getData = true;
                    }
                    if ((timeOuts * 50) >= TimeOut)
                    {
                        timeOut = true;
                    }
                    timeOuts++;
                }
                while (!timeOut && !getData);
                if (timeOut && !getData)
                {
                    Error = "读取数据超时,请检查连接";
                    result = false;
                }
                else
                {
                    byte[] readBuff = new byte[Com.BytesToRead];
                    Com.Read(readBuff, 0, readBuff.Length);
                    if (readBuff[0] != sendBuff[0] || readBuff[1] != sendBuff[1])
                    {
                        Error = "校验数据错误，返回的数据不正确";
                        result = false;
                    }
                    else
                    {
                        for (int i = 0; i < len; i++)
                        {
                            value[i] = readBuff[3 + i + curIndex];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Error = e.Message;
                result = false;
            }
            return result;
        }
        public bool ReadVW(int start, int len, out short[] value)
        {
            bool result = true;
            value = new short[len];
            try
            {
                int curStart = (int)(Math.Floor(start / 2.0f));
                int curIndex = 0;
                int curLen = len;
                if ((start % 2) == 1)
                {
                    curIndex = 1;
                    curLen = len + 1;
                }
                byte[] sendBuff = new byte[8];
                sendBuff[0] = Address;
                sendBuff[1] = 0x03;
                sendBuff[2] = (byte)(curStart >> 8);
                sendBuff[3] = (byte)(curStart & 0xFF);
                sendBuff[4] = (byte)(curLen >> 8);
                sendBuff[5] = (byte)(curLen & 0xFF);


                Crc16(sendBuff, 6, out sendBuff[6], out sendBuff[7]);

                //System.Windows.Forms.MessageBox.Show(Hex2Str(sendBuff, 0, sendBuff.Length));

                bool timeOut = false;
                bool getData = false;
                int timeOuts = 0;
                if (!Com.IsOpen)
                {
                    Com.Open();
                }
                Com.Write(sendBuff, 0, sendBuff.Length);
                do
                {
                    Thread.Sleep(50);
                    if (Com.BytesToRead >= curLen * 2 + 5)
                    {
                        getData = true;
                    }
                    if ((timeOuts * 50) >= TimeOut)
                    {
                        timeOut = true;
                    }
                    timeOuts++;
                }
                while (!timeOut && !getData);
                if (timeOut && !getData)
                {
                    Error = "读取数据超时,请检查连接";
                    result = false;
                }
                else
                {
                    byte[] readBuff = new byte[Com.BytesToRead];
                    Com.Read(readBuff, 0, readBuff.Length);
                    if (readBuff[0] != sendBuff[0] || readBuff[1] != sendBuff[1])
                    {
                        Error = "校验数据错误，返回的数据不正确";
                        result = false;
                    }
                    else
                    {
                        for (int i = 0; i < len; i++)
                        {
                            int tmp = (((readBuff[3 + curIndex + i * 2] << 8) + readBuff[4 + curIndex + i * 2]) & 0xFFFF);
                            if (tmp >= 0x8000)
                            {
                                value[i] = (short)(-((tmp ^ 0xFFFF) + 1));
                            }
                            else
                            {
                                value[i] = (short)(tmp);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Error = e.Message;
                result = false;
            }
            return result;
        }
        public static void Crc16(byte[] buff, int len, out byte crcLo, out byte crcHi)
        {
            crcLo = 0;
            crcHi = 0;
            if (len <= 0)
            {
                return;
            }
            int i, j;
            ushort maa = 0xFFFF;
            ushort mbb = 0;
            for (i = 0; i < len; i++)
            {
                crcHi = (byte)((maa >> 8) & 0xFF);
                crcLo = (byte)((maa) & 0xFF);
                maa = (ushort)((crcHi << 8) & 0xFF00);
                maa = (ushort)(maa + ((crcLo ^ buff[i]) & 0xFF));
                for (j = 0; j < 8; j++)
                {
                    mbb = 0;
                    mbb = (ushort)(maa & 0x1);
                    maa = (ushort)((maa >> 1) & 0x7FFF);
                    if (mbb != 0)
                    {
                        maa = (ushort)((maa ^ 0xA001) & 0xFFFF);
                    }
                }
            }
            crcLo = (byte)(maa & 0xFF);
            crcHi = (byte)((maa >> 8) & 0xFF);
        }
    }
}
