using PTPager.Alerting.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using PTPager.Alerting.Model;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace PTPager.Alerting.Polycom
{
    public class PolycomAudioTransmitter : IAudioTransmitter
    {
        const string CALLER_ID = "PTPager";

        public void Transmit(int channel, AudioInfo audioInfo)
        {
            Codec codec = Enum.Parse<Codec>(audioInfo.Codec.ToString());

            int actualChannel = channel + ChannelOffset;

            if (actualChannel<26 || actualChannel > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }

            Dictionary<uint, byte[]> data = ConvertAudioData(audioInfo);

            Send(actualChannel, CALLER_ID, data, codec);
        }

        private Dictionary<uint, byte[]> ConvertAudioData(AudioInfo audioInfo)
        {
            byte[] bytes = audioInfo.AudioData;

            Dictionary<uint, byte[]> speechAudioBytes = new Dictionary<uint, byte[]>();

            uint speechTimestamp = 1908944;

            List<byte> currentList = new List<byte>();
            for (int i = 0; i < bytes.Length; i++)
            {
                currentList.Add(bytes[i]);
                if (currentList.Count == 160)
                {
                    speechAudioBytes.Add(speechTimestamp, currentList.ToArray());
                    currentList = new List<byte>();
                    speechTimestamp += 160;
                }
            }
            if (currentList.Count > 0)
            {
                speechAudioBytes.Add(speechTimestamp, currentList.ToArray());
            }

            return speechAudioBytes;
        }

        const string address = "224.0.1.116";
        const int port = 5001;
        const int ChannelOffset = 25;

        private Stopwatch _stopwatch = new Stopwatch();

        internal void Send(int channelNumber, string callerId, Dictionary<uint, byte[]> audioData, Codec codec)
        {
            string mac = GetMacAddress();

            string hostSerialNumber = string.Format("{0}{1}-{2}{3}-{4}{5}-{6}{7}", mac[4], mac[5], mac[6], mac[7], mac[8], mac[9], mac[10], mac[11]);

            {
                PolycomPTTPacket alertPacket = new PolycomPTTPacket();
                alertPacket.OpCode = OpCode.Alert;
                alertPacket.ChannelNumber = channelNumber;
                alertPacket.CallerID = callerId;
                alertPacket.HostSerialNumber = hostSerialNumber;

                for (int i = 0; i < 31; i++)
                {
                    //Console.WriteLine("Alert");
                    byte[] packetData = alertPacket.ToPacket();

                    if (i > 0) WaitFor(30);

                    SendPacket(packetData);
                }
            }

            byte[] previousAudioData = null;

            foreach (var audioDataItem in audioData)
            {
                PolycomPTTPacket audioPacket = new PolycomPTTPacket();
                audioPacket.OpCode = OpCode.Transmit;
                audioPacket.ChannelNumber = channelNumber;
                audioPacket.CallerID = callerId;
                audioPacket.HostSerialNumber = hostSerialNumber;

                audioPacket.Codec = codec;
                audioPacket.Flags = "00";
                audioPacket.SampleCount = audioDataItem.Key;

                audioPacket.AudioData = (byte[])audioDataItem.Value.Clone();
                audioPacket.PreviousAudioData = previousAudioData;

                //Console.WriteLine("Transmit");
                byte[] packetData = audioPacket.ToPacket();

                WaitFor(20);

                SendPacket(packetData);
                previousAudioData = (byte[])audioPacket.AudioData.Clone();
            }

            WaitFor(50);

            {
                PolycomPTTPacket endPacket = new PolycomPTTPacket();
                endPacket.OpCode = OpCode.EndOfTransmit;
                endPacket.ChannelNumber = channelNumber;
                endPacket.CallerID = callerId;
                endPacket.HostSerialNumber = hostSerialNumber;

                for (int i = 0; i < 12; i++)
                {
                    //Console.WriteLine("EndOfTransmit");
                    byte[] packetData = endPacket.ToPacket();

                    if (i > 0) WaitFor(30);

                    SendPacket(packetData);
                }

            }


        }

        private void WaitFor(int milliseconds)
        {
            while (_stopwatch.ElapsedMilliseconds < milliseconds)
            {
                Thread.Sleep(1);
            }
        }

        private void SendPacket(byte[] packet)
        {
            _stopwatch.Restart();
            //return;
            using (Socket mSendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                mSendSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                                            new MulticastOption(IPAddress.Parse(address)));
                mSendSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 255);
                mSendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                mSendSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(address), port);
                mSendSocket.Connect(ipep);


                byte[] bytes = packet;
                mSendSocket.Send(bytes, bytes.Length, SocketFlags.None);
            }
        }

        private string GetMacAddress()
        {
            const int MIN_MAC_ADDR_LENGTH = 12;
            string macAddress = string.Empty;
            long maxSpeed = -1;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                //log.Debug(
                //    "Found MAC Address: " + nic.GetPhysicalAddress() +
                //    " Type: " + nic.NetworkInterfaceType);

                string tempMac = nic.GetPhysicalAddress().ToString();
                if (nic.Speed > maxSpeed &&
                    !string.IsNullOrEmpty(tempMac) &&
                    tempMac.Length >= MIN_MAC_ADDR_LENGTH)
                {
                    //log.Debug("New Max Speed = " + nic.Speed + ", MAC: " + tempMac);
                    maxSpeed = nic.Speed;
                    macAddress = tempMac;
                }
            }

            return macAddress;
        }
    }
}
