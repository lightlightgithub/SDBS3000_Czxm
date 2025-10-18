using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBS3000.BalTest
{
    /// PRG键进入设置项
    /// P0.02:0变频器键盘控制启停  1端子控制  2上位机通讯控制
    /// P0.04:2FIV控制 4键盘控制  9上位机通讯控制
    /// P0.10:最大频率
    /// 公司一般设置为端子控制、FIV控制，上位机测试需改设置项，测试完改回端子、FIV控制
    public class Inverter
    {
        public enum CtrlCode : ushort
        {
            Forward = 0x0001,
            Reverse = 0x0002,
            JogForward = 0x0003,
            JogReverse = 0x0004,
            Stop = 0x0005,
            Reduce = 0x0006,
            AlarmReset = 0x0007,
        }

        const byte ADRESS = 0x01;

        const double MAX_FREQUENCY = 100;

        private readonly SerialPort _port;

        public bool IsOpen => _port.IsOpen;

        public Inverter(string portName)
        {
            _port = new SerialPort
            {
                PortName = portName,
                BaudRate = 9600,
                Parity = Parity.None,
            };
        }

        public void Open()
        {
            _port.Open();

        }

        public void SetFrequency(double frequency)
        {
            const ushort WRITE_ADRESS = 0x1000;

            var data = new byte[8];
            var frame = data.AsSpan();
            frame[0] = ADRESS;
            frame[1] = 6;

            BinaryPrimitives.WriteUInt16BigEndian(frame.Slice(2), WRITE_ADRESS);
            BinaryPrimitives.WriteInt16BigEndian(frame.Slice(4), (short)(frequency / MAX_FREQUENCY * 10000));

            var crc = Crc(frame.Slice(0, 6));
            BinaryPrimitives.WriteUInt16LittleEndian(frame.Slice(6), crc);

            _port.BaseStream.Write(data, 0, data.Length);
        }

        public void SetRpm(double rpm)
        {
            const double SCALE = 185.185;
            SetFrequency(rpm / SCALE);
        }

        public void SetCtrlCode(CtrlCode code)
        {
            const ushort WRITE_ADRESS = 0x2000;

            var data = new byte[8];
            var frame = data.AsSpan();
            frame[0] = ADRESS;
            frame[1] = 6;

            BinaryPrimitives.WriteUInt16BigEndian(frame.Slice(2), WRITE_ADRESS);
            BinaryPrimitives.WriteUInt16BigEndian(frame.Slice(4), (ushort)(code));

            var crc = Crc(frame.Slice(0, 6));
            BinaryPrimitives.WriteUInt16LittleEndian(frame.Slice(6), crc);

            _port.BaseStream.Write(data, 0, data.Length);
        }

        private static ushort Crc(Span<byte> data)
        {
            ushort crc = 0xffff;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xa001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return crc;
        }
    }
}
