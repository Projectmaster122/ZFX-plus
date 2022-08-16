using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ZFX
{
    class CPU
    {
        ///<summary>
        ///System initted
        //</summary>
        public bool init = false;
        /// <summary>
        /// RAM array
        /// </summary>
        public long[] RAM { get; private set; }
        ///<summary>
        ///Size of RAM in KB.
        ///</summary>
        public long bitsize { get; private set; }
        /// <summary>
        /// The List of reserved indexes
        /// </summary>
        public long?[] RIL;

        /// <summary>
        /// Flags for CPU.cs. MUST SET EXACTLY AFTER INITIALIZATION. NO CALLS BEFOREHAND
        /// </summary>
        public Field Field;

        public long dreg1;
        public long dreg2;
        public int pcounter;

        /// <summary>
        /// Set DREG1 register data.
        /// </summary>
        /// <param name="Value">Value to set DREG1 to</param>
        public void SetDreg1Data(long Value)
        {
            dreg1 = Value;
        }

        /// <summary>
        /// Get DREG1 register data.
        /// </summary>
        /// <returns>Value of DREG1 (long)</returns>
        public long GetDreg1Data()
        {
            return dreg1;
        }

        /// <summary>
        /// Set DREG2 register data.
        /// </summary>
        /// <param name="Value">Value to set DREG2 to.</param>
        public void SetDreg2Data(long Value)
        {
            dreg2 = Value;
        }

        /// <summary>
        /// Get DREG2 register data.
        /// </summary>
        /// <returns>Value of DREG2 data.</returns>
        public long GetDreg2Data()
        {
            return dreg2;
        }
        ///<summary>
        ///Halt the system.
        ///</summary>
        public void hlt()
        {
            memclean(0, bitsize);
            RAM = null;
            Field.WriteDebug("Halted CPU! Goodbye, World.");
            while (true) { }
        }
        ///<summary>
        ///Clean memory from startIndex to endIndex
        ///<paramref name="startIndex">Start of index to clean</paramref>
        ///<paramref name="endIndex">End index to clean</paramref>
        ///</summary>
        public void memclean(int startIndex, long endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                setMemLoc(i, 0);
            }
        }

        /// <summary>
        /// Read index.
        /// </summary>
        /// <param name="index">Index to read</param>
        /// <returns>Value of index (int)</returns>
        public long ReadIndex(long index)
        {
            if (index < 0 || index > bitsize) { return 0; }
            return RAM[index];
        }
        /// <summary>
        /// Reserves a certain index
        /// </summary>
        /// <param name="i">The index to reserve</param>
        public void ReserveIndex(long i)
        {
            if (i >= bitsize)
            {
                return;
            }
            RIL[i] = RAM[i];
            RAM[i] = 0;
        }
        /// <summary>
        /// Unreserves an index
        /// </summary>
        /// <param name="i">The index to unreserve</param>
        public void UnreserveIndex(long i)
        {
            long? a = RIL[i];
            RIL[i] = null;
            RAM[i] = (long)a;
        }
        /// <summary>
        /// Checks if a certain index is reserved
        /// </summary>
        /// <param name="i">the index to check</param>
        /// <returns></returns>
        public bool IsReserved(long i)
        {
            return RIL[i] == 0;
        }
        /// <summary>
        /// Reads an array of indexes
        /// </summary>
        public long[] ReadIndexArray(long sindex, long eindex)
        {
            long[] tmp = new long[eindex];
            for (long i = sindex; i < eindex; i++)
            {
                Field.WriteDebug("Read indexes from " + sindex + "to " + eindex);
                tmp[i] = RAM[i];
            }
            return tmp;
        }
        /// <summary>
        /// Pointer to the display class
        /// </summary>
        public Display Display;
        /// <summary>
        /// Reads from starting index untill the end of the RAM (unless its 0, use at your own risk)
        /// </summary>
        /// <param name="sindex">Starting index, Default: 0</param>
        /// <returns></returns>
        public long[] rd(long sindex = 0)
        {
            var tmp = new List<long>();
            for (long i = sindex; i < bitsize; i++)
            {
                if (RAM[i] == 0)
                {
                    break;
                }
                Field.WriteDebug("Read indexes from " + sindex + "to nullbyte");    
                tmp.Add(RAM[i]);
            }
            return tmp.ToArray();
        }
        ///<summary>
        ///Copy memory.
        ///</summary>
        ///<param name="from">Index to copy</param>
        ///<param name="to">Index to copy to</param>
        public void memcpy(long from, long to)
        {
            if (from >= bitsize || to >= bitsize)
            {
                return;
            }
            RAM[to] = RAM[from];
        }
        ///<summary>
        ///Sets a memory location
        ///</summary>
        ///<param name="index">Index to set</param>
        ///<param name="val">Value</param>
        public void setMemLoc(long index, int val)
        {
            if (index >= bitsize || IsReserved(index))
            {
                return;
            }
            this.RAM[index] = val;
        }
        private void setReservedBit(long index, int val)
        {
            if (index >= bitsize)
            {
                return;
            }
            Field.WriteDebug("Wrote " + val + "to " + index);
            RAM[index] = val;
        }
        ///<summary>
        ///Increment index by 1
        ///</summary>
        ///<param name="index">Index to increment</param>
        public void inc(int index)
        {
            if (index >= bitsize)
            {
                return;
            }
            Field.WriteDebug("Incremented " + index);
            RAM[index]++;
        }
        ///<summary>
        ///Dump memory to screen.
        ///</summary>
        public void memdmp()
        {
            for (int i = 0; i < bitsize; i++)
            {
                Console.Write((char)RAM[i]);
            }
            Console.Write('\n');
        }
        ///<summary>
        ///Dump memory as int
        ///</summary>
        ///<param name="e">Whether or not to print 0's (default: true)</param>
        public void intdmp(bool e = true)
        {
            for (int i = 0; i < RAM.Length; i++)
            {
                if (RAM[i] == 0 && !e) { return; }
                Console.Write(RAM[i].ToString() + " ");
            }
            Console.Write('\n');
        }
        /// <summary>
        /// Clear the screen, really really useful
        /// </summary>
        public void clear()
        {
            Console.Clear();
        }
        /// <summary>
        /// Compare indexes
        /// </summary>
        /// <param name="l1">n1</param>
        /// <param name="l2">n2</param>
        /// <param name="wh">Where to save (1 or 0)</param>
        public void memcmp(int l1, int l2, int wh)
        {
            setMemLoc(wh, l1 == l2 ? 1 : 0);
            Field.WriteDebug("Compared " + l1 + " with " + l2 + " and got " + (l1 == l2).ToString());
        }
        ///<summary>
        ///Swap indexes.
        ///</summary>
        ///<param name="i1">Index 1</param>
        ///<param name="i2">Index 2</param>
        public void swp(int i1, int i2)
        {
            RAM[i1] = RAM[i1] + RAM[i2];
            RAM[i2] = RAM[i1] - RAM[i2];
            RAM[i1] = RAM[i1] - RAM[i2];
            Field.WriteDebug("Swapped " + i1 + " with " + i2);
        }
        ///<summary>
        ///Read to memory
        ///</summary>
        ///<param name="startIndex">Index to start saving to (default:0)</param>
        public void rde(string a, long startIndex = 0)
        {
            for (int i = 0; i < a.Length; i++)
            {
                setMemLoc(i + startIndex, a[i]);
            }
            Field.WriteDebug("Received string a " + a + "and storing at " + startIndex);
        }

        /// <summary>
        /// Print memory locations 
        /// </summary>
        /// <param name="from">From (default:0)</param>
        /// <param name="to">To (default:0)</param>
        public void prnt(int from = 0, int to = 0)
        {
            // function use
            {
                if (to == 0)
                {
                    for (int i = from; i < bitsize; i++)
                    {
                        if (RAM[i] == 0)
                        {
                            to = i;
                            break;
                        }
                    }
                }
                for (int i = from; i < to; i++)
                {
                    if (RAM[i] > 255)
                    {
                        Debug.WriteLine("?");
                    }
                    else
                        Debug.WriteLine((char)RAM[i]);
                }
            }
        }
        /// <summary>
        /// Sum
        /// </summary>
        /// <param name="l1">n1</param>
        /// <param name="l2">n2</param>
        /// <param name="wh">Index to store</param>
        public void add(int l1, int l2, int wh)
        {
            setMemLoc(wh, l1 + l2);
        }
        /// <summary>
        /// Sub
        /// </summary>
        /// <param name="l1">n1</param>
        /// <param name="l2">n2</param>
        /// <param name="wh">Index to store</param>
        public void sub(int l1, int l2, int wh)
        {
            setMemLoc(wh, l1 - l2);
        }
        /// <summary>
        /// Mul
        /// </summary>
        /// <param name="l1">n1</param>
        /// <param name="l2">n2</param>
        /// <param name="wh">Index to store</param>
        public void mul(int l1, int l2, int wh)
        {
            setMemLoc(wh, l1 * l2);
        }
        /// <summary>
        /// Div
        /// </summary>
        /// <param name="l1">n1</param>
        /// <param name="l2">n2</param>
        /// <param name="wh">Index to store</param>
        public void div(int l1, int l2, int wh)
        {
            setMemLoc(wh, Convert.ToInt32(Math.Round((double)(l1 / l2))));
        }
        /// <summary>
        /// Square root
        /// </summary>
        /// <param name="l1">n1</param>
        /// <param name="l2">n2</param>
        /// <param name="wh">Index to store</param>
        public void sqrt(int l, int wh)
        {
            setMemLoc(wh, Convert.ToInt32(Math.Round(Math.Sqrt(l))));
        }
        /// <summary>
        /// Power of
        /// </summary>
        /// <param name="l1">n1</param>
        /// <param name="l2">n2</param>
        /// <param name="wh">Index to store</param>
        public void pow(int l1, int l2, int wh)
        {
            setMemLoc(wh, Convert.ToInt32(Math.Round(Math.Pow(l1, l2))));
        }
        /// <summary>
        /// Init system
        /// </summary>
        /// <param name="bitSystem">Size of RAM in KB</param>
        public CPU(long bitSystem = 2)
        {
            initd(bitSystem * 1024);
        }
        /// <summary>
        /// Enum for every panic scenario
        /// </summary>
        public enum PanicType { criticalerror, gp, matherror }
        /// <summary>
        /// Init function, Can only be run once. USE AT YOUR OWN RISK!
        /// </summary>
        /// <param name="memsize">Amount of memory to allocate</param>
        public void initd(long memsize)
        {
            if (init)
            {
                Environment.Exit(1);
            }
            // memtemp init
            {
                long memtemp = memsize;
                memtemp /= 1024;
                {
                    if (memtemp / 1024 >= 1)
                    {
                        memtemp /= 1024;
                    }
                    if (memtemp / 1024 >= 1)
                    {
                        memtemp /= 1024;
                    }
                    if (memtemp <= 0)
                    {
                        Console.WriteLine("Not enough memory to load software.");
                        Environment.Exit(1);
                    }
                }

            }
            // array init
            {
                RAM = new long[memsize];
                bitsize = memsize;
                {
                    RIL = new long?[memsize];
                    for (int i = 0; i < memsize; i++)
                    {
                        RIL[i] = null;
                    }
                    memclean(0, bitsize);
                }

 
            }

            init = true;
        }
    }
}