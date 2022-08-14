using System;
using System.Collections.Generic;

namespace ZFX
{
    public class CPU
    {
        public Flags Flags;
        ///<summary>
        ///RAM array.
        ///</summary>
        public bool init = false;
        /// <summary>
        /// Locator to network
        /// </summary>
        public Network Network;
        /// <summary>
        /// RAM array
        /// </summary>
        public long[] RAM { get; private set; } 
        ///<summary>
        ///Size of RAM in KB.
        ///</summary>
        public long bitsize { get; private set; }
        /// <summary>
        /// Current ring. Before init is true this is set to null
        /// </summary>
        public int? ring = null;
        /// <summary>
        /// The List of reserved indexes
        /// </summary>
        public long?[] RIL;
        /// <summary>
        /// Print a string to the debug console
        /// </summary>
        /// <paramref name="Message">Message to write</param>
        public void WriteDebug(string Message)
        {
            if (Flags.DebugMessages)
            {
                prnt("[DEBUG] " + Message);
            }
        }
        ///<summary>
        ///Halt the system.
        ///</summary>
        public void hlt()
        {
            memclean(0, bitsize);
            RAM = null;
            WriteDebug("Halted CPU! Goodbye, World.");
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
        public void panic(PanicType panicType)
        {
            Console.Clear();
            string pMsg = "";
            switch (panicType)
            {
                case PanicType.criticalerror:
                    pMsg = "Critical Error";
                    break;
                case PanicType.gp:
                    pMsg = "General Protection Fault";
                    break;
                case PanicType.matherror:
                    pMsg = "Math Error";
                    break;
                case PanicType.permdenied:
                    pMsg = "Permission Denied";
                    break;
            }
            Console.WriteLine("Error during execution\n{0}", pMsg);
            memclean(0, bitsize);
            Console.ReadLine();
            Environment.Exit(1);
        }
        public int rdI(int index)
        {
            if (index < 0 || index > bitsize) { panic(PanicType.gp); return 0; }
            WriteDebug("Read index " + index);
            return RAM[index];
        }
        public int[] rd(int sindex, int eindex)
        {
            int[] tmp = new int[eindex];
            for (int i = sindex; i < eindex; i++)
            {
                tmp[i] = RAM[i];
            }
            WriteDebug("Read indexes from " + sindex + "to " + eindex);
            return tmp;
        }
        public Display Display;
        public int[] rd(int sindex = 0)
        {
            var tmp = new List<int>();
            for (int i = sindex; i < bitsize; i++)
            {
                if (RAM[i] == 0)
                {
                    break;
                }
                tmp.Add(RAM[i]);
            }
            WriteDebug("Read indexes from " + sindex + "to nullbyte");
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
                panic(PanicType.gp);
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
            if (index >= bitsize)
            {
                panic(PanicType.gp);
                return;
            }
            WriteDebug("Wrote " + val + "to " + index);
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
                panic(PanicType.gp);
                return;
            }
            WriteDebug("Incremented " + index);
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

        public void clear()
        {
            Console.Clear(); //very useful, in fact all of Z depends on this
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
            WriteDebug("Compared " + l1 + " with " + l2 + " and got " + (l1 == l2).ToString());
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
            WriteDebug("Swapped " + i1 + "with " + i2);
        }
        ///<summary>
        ///Read to memory
        ///</summary>
        ///<param name="startIndex">Index to start saving to (default:0)</param>
        public void rde(long startIndex = 0)
        {
            string a = Console.ReadLine();
            for (int i = 0; i < a.Length; i++)
            {
                setMemLoc(i + startIndex, a[i]);
            }
            WriteDebug("Read input " + a + "and storing at " + startIndex);
        }
        /// <summary>
        /// Print string and save it to memory (from 0 to length of string)
        /// </summary>
        /// <param name="pr">String to print</param>
        public void prnt(string pr)
        {
            for (int i = 0; i < pr.Length; i++)
            {
                setMemLoc(i, (int)pr[i]);
            }
            for (int i = 0; i < pr.Length; i++)
            {
                Console.Write((char)RAM[i]);
            }
            memclean(0, pr.Length);
        }
        public void nl()
        {
            Console.Write(Environment.NewLine);//Z depends on this too
        }
        /// <summary>
        /// Print memory locations 
        /// </summary>
        /// <param name="from">From (default:0)</param>
        /// <param name="to">To (default:0)</param>
        public void prnt(int from = 0, int to = 0)
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
                Console.Write((char)RAM[i]);
            }
            Console.Write('\n');
        }
        /// <summary>
        /// Sum
        /// </summary>
        /// <param name="l1">n1</param>
        /// <param name="l2">n2</param>
        /// <param name="wh">Index to store</param>
        public void add(int l1, int l2, int wh)
        {
            setMemLoc(wh, l1 + l2); //MATH REGISTERS NOW!
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
            setMemLoc(wh, Convert.ToInt32(Math.Round((double)(l1 / l2)))); //MATH REGISTERS!!!!!!!!!!
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
        public enum PanicType { criticalerror, gp, matherror, permdenied }
        public void initd(long memsize)
        {
            Flags = new Flags();
            Flags.DebugMessages = false;
            if (init)
            {
                panic(PanicType.criticalerror);
                Environment.Exit(1);
            }
            long memtemp = memsize;
            memtemp /= 1024;
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
            RAM = new int[memsize];
            bitsize = memsize;
            memclean(
                0, bitsize);
            init = true;
        }
    }
}