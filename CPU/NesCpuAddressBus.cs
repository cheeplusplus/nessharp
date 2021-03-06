using System;
using NesSharp.Memory;

namespace NesSharp.CPU
{
    public class NesCpuAddressBus : IMemoryAccess
    {
        private Nes Nes;

        public NesCpuAddressBus(Nes nes)
        {
            Nes = nes;
        }

        public byte ReadByte(ushort address, bool quiet = false)
        {
            if (InMapperRange(address))
            {
                return Nes.Cart.Mapper.CpuReadByte(address, quiet);
            }
            else if (address >= 0x2000 && address <= 0x3FFF)
            {
                // 0x2000-0x2007 are mirrored every 8 bytes
                ushort offset = (ushort)(address % 8);
                return Nes.Ppu.Read(offset);
            }
            else if (address >= 0x4000 && address <= 0x4017)
            {
                return 0xFF;
                // throw new Exception("APU not yet implemented");
            }
            else
            {
                return Nes.Cpu.Memory.ReadByte(address, quiet);
            }
        }

        public ushort ReadAddress(ushort address, bool quiet = false)
        {
            if (InMapperRange(address))
            {
                return Nes.Cart.Mapper.CpuReadUShort(address, quiet);
            }
            else if (address >= 0x2000 && address <= 0x3FFF)
            {
                throw new IllegalMemoryAccessException(AddressBus.CPU, address, "Attempted to read 16 bits from PPU registers");
            }
            else if (address >= 0x4000 && address <= 0x4017)
            {
                throw new IllegalMemoryAccessException(AddressBus.CPU, address, "Attempted to read 16 bits from APU/status registers");
            }
            else
            {
                return Nes.Cpu.Memory.ReadAddress(address, quiet);
            }
        }

        public void Write(ushort address, byte value)
        {
            if (InMapperRange(address))
            {
                Nes.Cart.Mapper.CpuWrite(address, value);
            }
            else if (address >= 0x2000 && address <= 0x3FFF)
            {
                // 0x2000-0x2007 are mirrored every 8 bytes
                ushort offset = (ushort)(address % 8);
                Nes.Ppu.Write(offset, value);
            }
            else if (address >= 0x4000 && address <= 0x4013)
            {
                // TODO: APU
            }
            else if (address >= 0x4014 && address <= 0x4015)
            {
                // TODO: OAM-DMA
            }
            else if (address >= 0x4016 && address <= 0x4017)
            {
                // TODO: Joystick handling
            }
            else
            {
                Nes.Cpu.Memory.Write(address, value);
            }
        }

        protected bool InMapperRange(ushort address)
        {
            if (address >= Nes.Cart.Mapper.CPUStartRange && address <= Nes.Cart.Mapper.CPUEndRange)
            {
                return true;
            }

            return false;
        }
    }
}