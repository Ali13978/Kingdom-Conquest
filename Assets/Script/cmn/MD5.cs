using System;

class md5_state_t 
{
    public uint[] count    = new uint[2];	/* message length in bits, lsw first */
    public uint[] abcd     = new uint[4];		/* digest buffer */
    public byte[] buf      = new byte[64];		/* accumulate block */
};

class MD5
{
    public byte[] CalcMD5(byte[] data)
    {
        md5_init();
        md5_append(data);
        md5_finish();

        return md5value;
    }
    public string CalcMD5String(byte[] data)
    {
        string s = "";
        byte[] bytes = CalcMD5(data);
        for (int i = 0; i < bytes.Length; i++)
        {
            s += System.Convert.ToString(bytes[i], 16).PadLeft(2, '0');
        }
        return s;
    }

    // 下面的都是见不得人的  
    md5_state_t ms          = new md5_state_t();
    byte[]      md5value    = new byte[16];

    // Initialize the algorithm.
    void md5_init()
    {
        ms.count[0] = ms.count[1] = 0;
        ms.abcd[0] = 0x67452301;
        ms.abcd[1] = /*0xefcdab89*/ T_MASK ^ 0x10325476;
        ms.abcd[2] = /*0x98badcfe*/ T_MASK ^ 0x67452301;
        ms.abcd[3] = 0x10325476;
    }

    // Append a string to the message.
    void md5_append(byte[] data)
    {
        int left = data.Length;
        int offsetSrc = 0;
        int offset = (int)((ms.count[0] >> 3) & 63);
        uint nbits = (uint)(data.Length << 3);

        if (data.Length <= 0)
            return;

        /* Update the message length. */
        ms.count[1] += (uint)(data.Length >> 29);
        ms.count[0] += nbits;
        if (ms.count[0] < nbits)
            ms.count[1]++;

        /* Process an initial partial block. */
        if (offset > 0)
        {
            int copy = (offset + data.Length > 64 ? 64 - offset : data.Length);

            bytesCopy(ms.buf, offset, data, offsetSrc, copy);
            if (offset + copy < 64)
                return;
            offsetSrc += copy;
            left -= copy;
            md5_process(ms.buf, 0);
        }

        /* Process full blocks. */
        for (; left >= 64; offsetSrc += 64, left -= 64)
            md5_process(data, offsetSrc);

        /* Process a final partial block. */
        if (left > 0)
            bytesCopy(ms.buf, 0, data, offsetSrc, left);
    }

    void bytesCopy(byte[] dst, int offsetDst, byte[] src, int offsetSrc, int length)
    {
        for (int i = 0; i < length; i++)
        {
            dst[i + offsetDst] = src[i + offsetSrc];
        }
    }

    // Finish the message and return the digest.
    void md5_finish()
    {
	    byte[] pad = 
	    {
		    0x80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };
        byte[] data	= new byte[8];
        int i;
        /* Save the length before padding. */
        for (i = 0; i < 8; ++i)
		    data[i] = (byte)(ms.count[i >> 2] >> ((i & 3) << 3));
        /* Pad to 56 bytes mod 64. */
	    uint	padSize	= ((55 - (ms.count[0] >> 3)) & 63) + 1;
	    byte[]	newPad	= new byte[padSize];
	    bytesCopy(newPad, 0, pad, 0, (int)padSize);
	    md5_append(newPad);
        /* Append the length. */
        md5_append(data);
        for (i = 0; i < 16; ++i)
            md5value[i] = (byte)(ms.abcd[i >> 2] >> ((i & 3) << 3));
    }

    const uint T_MASK = ((uint)0xFFFFFFFF);
    const uint T1 = (T_MASK ^ 0x28955b87);
    const uint T2 = (T_MASK ^ 0x173848a9);
    const uint T3 = 0x242070db;
    const uint T4 = (T_MASK ^ 0x3e423111);
    const uint T5 = (T_MASK ^ 0x0a83f050);
    const uint T6 = 0x4787c62a;
    const uint T7 = (T_MASK ^ 0x57cfb9ec);
    const uint T8 = (T_MASK ^ 0x02b96afe);
    const uint T9 = 0x698098d8;
    const uint T10 = (T_MASK ^ 0x74bb0850);
    const uint T11 = (T_MASK ^ 0x0000a44e);
    const uint T12 = (T_MASK ^ 0x76a32841);
    const uint T13 = 0x6b901122;
    const uint T14 = (T_MASK ^ 0x02678e6c);
    const uint T15 = (T_MASK ^ 0x5986bc71);
    const uint T16 = 0x49b40821;
    const uint T17 = (T_MASK ^ 0x09e1da9d);
    const uint T18 = (T_MASK ^ 0x3fbf4cbf);
    const uint T19 = 0x265e5a51;
    const uint T20 = (T_MASK ^ 0x16493855);
    const uint T21 = (T_MASK ^ 0x29d0efa2);
    const uint T22 = 0x02441453;
    const uint T23 = (T_MASK ^ 0x275e197e);
    const uint T24 = (T_MASK ^ 0x182c0437);
    const uint T25 = 0x21e1cde6;
    const uint T26 = (T_MASK ^ 0x3cc8f829);
    const uint T27 = (T_MASK ^ 0x0b2af278);
    const uint T28 = 0x455a14ed;
    const uint T29 = (T_MASK ^ 0x561c16fa);
    const uint T30 = (T_MASK ^ 0x03105c07);
    const uint T31 = 0x676f02d9;
    const uint T32 = (T_MASK ^ 0x72d5b375);
    const uint T33 = (T_MASK ^ 0x0005c6bd);
    const uint T34 = (T_MASK ^ 0x788e097e);
    const uint T35 = 0x6d9d6122;
    const uint T36 = (T_MASK ^ 0x021ac7f3);
    const uint T37 = (T_MASK ^ 0x5b4115bb);
    const uint T38 = 0x4bdecfa9;
    const uint T39 = (T_MASK ^ 0x0944b49f);
    const uint T40 = (T_MASK ^ 0x4140438f);
    const uint T41 = 0x289b7ec6;
    const uint T42 = (T_MASK ^ 0x155ed805);
    const uint T43 = (T_MASK ^ 0x2b10cf7a);
    const uint T44 = 0x04881d05;
    const uint T45 = (T_MASK ^ 0x262b2fc6);
    const uint T46 = (T_MASK ^ 0x1924661a);
    const uint T47 = 0x1fa27cf8;
    const uint T48 = (T_MASK ^ 0x3b53a99a);
    const uint T49 = (T_MASK ^ 0x0bd6ddbb);
    const uint T50 = 0x432aff97;
    const uint T51 = (T_MASK ^ 0x546bdc58);
    const uint T52 = (T_MASK ^ 0x036c5fc6);
    const uint T53 = 0x655b59c3;
    const uint T54 = (T_MASK ^ 0x70f3336d);
    const uint T55 = (T_MASK ^ 0x00100b82);
    const uint T56 = (T_MASK ^ 0x7a7ba22e);
    const uint T57 = 0x6fa87e4f;
    const uint T58 = (T_MASK ^ 0x01d3191f);
    const uint T59 = (T_MASK ^ 0x5cfebceb);
    const uint T60 = 0x4e0811a1;
    const uint T61 = (T_MASK ^ 0x08ac817d);
    const uint T62 = (T_MASK ^ 0x42c50dca);
    const uint T63 = 0x2ad7d2bb;
    const uint T64 = (T_MASK ^ 0x14792c6e);

    uint ROTATE_LEFT(uint x, byte n)
	{
		return ((x) << (n)) | ((x) >> (32 - (n)));
	}
	
	uint F(uint x, uint y, uint z)
	{
		return (((x) & (y)) | (~(x) & (z)));
	}
	
	void SET_F(ref uint a, uint b, uint c, uint d, uint k, byte s, uint Ti, ref uint t, uint[] X)
	{
		t = a + F(b,c,d) + X[k] + Ti;
		a = ROTATE_LEFT(t, s) + b;
	}
	
	uint G(uint x, uint y, uint z)
	{
		return (((x) & (z)) | ((y) & ~(z)));
	}

    void SET_G(ref uint a, uint b, uint c, uint d, uint k, byte s, uint Ti, ref uint t, uint[] X)
	{
		t = a + G(b,c,d) + X[k] + Ti;
		a = ROTATE_LEFT(t, s) + b;
	}
  
	uint H(uint x, uint y, uint z)
	{
		return ((x) ^ (y) ^ (z));
	}

    void SET_H(ref uint a, uint b, uint c, uint d, uint k, byte s, uint Ti, ref uint t, uint[] X)
	{
		t = a + H(b,c,d) + X[k] + Ti;
		a = ROTATE_LEFT(t, s) + b;
	}
	
	uint I(uint x, uint y, uint z)
	{
		return ((y) ^ ((x) | ~(z)));
	}

    void SET_I(ref uint a, uint b, uint c, uint d, uint k, byte s, uint Ti, ref uint t, uint[] X)
	{
		t = a + I(b,c,d) + X[k] + Ti;
		a = ROTATE_LEFT(t, s) + b;
	}
	
    void	md5_process(byte[] data, int offset)
    {
        uint
	    a = ms.abcd[0], b = ms.abcd[1],
	    c = ms.abcd[2], d = ms.abcd[3];
        uint t  = 0;
	
        /* Define storage for little-endian or both types of CPUs. */
        uint[] xbuf	= new uint[16];
        for (int i=0; i<16; i++)
	    {
            xbuf[i] = (uint)(data[0 + i * 4 + offset] | data[1 + i * 4 + offset] << 8 | data[2 + i * 4 + offset] << 16 | data[3 + i * 4 + offset] << 24);
	    }

        /* Round 1. */
        /* Let [abcd k s i] denote the operation
           a = b + ((a + F(b,c,d) + X[k] + T[i]) <<< s). */

        /* Do the following 16 operations. */
        SET_F(ref a, b, c, d,  0,  7,  T1, ref t, xbuf);
        SET_F(ref d, a, b, c,  1, 12,  T2, ref t, xbuf);
        SET_F(ref c, d, a, b,  2, 17,  T3, ref t, xbuf);
        SET_F(ref b, c, d, a,  3, 22,  T4, ref t, xbuf);
        SET_F(ref a, b, c, d,  4,  7,  T5, ref t, xbuf);
        SET_F(ref d, a, b, c,  5, 12,  T6, ref t, xbuf);
        SET_F(ref c, d, a, b,  6, 17,  T7, ref t, xbuf);
        SET_F(ref b, c, d, a,  7, 22,  T8, ref t, xbuf);
        SET_F(ref a, b, c, d,  8,  7,  T9, ref t, xbuf);
        SET_F(ref d, a, b, c,  9, 12, T10, ref t, xbuf);
        SET_F(ref c, d, a, b, 10, 17, T11, ref t, xbuf);
        SET_F(ref b, c, d, a, 11, 22, T12, ref t, xbuf);
        SET_F(ref a, b, c, d, 12,  7, T13, ref t, xbuf);
        SET_F(ref d, a, b, c, 13, 12, T14, ref t, xbuf);
        SET_F(ref c, d, a, b, 14, 17, T15, ref t, xbuf);
        SET_F(ref b, c, d, a, 15, 22, T16, ref t, xbuf);

         /* Round 2. */
         /* Let [abcd k s i] denote the operation
              a = b + ((a + G(b,c,d) + X[k] + T[i]) <<< s). */
         /* Do the following 16 operations. */
        SET_G(ref a, b, c, d,  1,  5, T17, ref t, xbuf);
        SET_G(ref d, a, b, c,  6,  9, T18, ref t, xbuf);
        SET_G(ref c, d, a, b, 11, 14, T19, ref t, xbuf);
        SET_G(ref b, c, d, a,  0, 20, T20, ref t, xbuf);
        SET_G(ref a, b, c, d,  5,  5, T21, ref t, xbuf);
        SET_G(ref d, a, b, c, 10,  9, T22, ref t, xbuf);
        SET_G(ref c, d, a, b, 15, 14, T23, ref t, xbuf);
        SET_G(ref b, c, d, a,  4, 20, T24, ref t, xbuf);
        SET_G(ref a, b, c, d,  9,  5, T25, ref t, xbuf);
        SET_G(ref d, a, b, c, 14,  9, T26, ref t, xbuf);
        SET_G(ref c, d, a, b,  3, 14, T27, ref t, xbuf);
        SET_G(ref b, c, d, a,  8, 20, T28, ref t, xbuf);
        SET_G(ref a, b, c, d, 13,  5, T29, ref t, xbuf);
        SET_G(ref d, a, b, c,  2,  9, T30, ref t, xbuf);
        SET_G(ref c, d, a, b,  7, 14, T31, ref t, xbuf);
        SET_G(ref b, c, d, a, 12, 20, T32, ref t, xbuf);

         /* Round 3. */
         /* Let [abcd k s t] denote the operation
              a = b + ((a + H(b,c,d) + X[k] + T[i]) <<< s). */
         /* Do the following 16 operations. */
        SET_H(ref a, b, c, d,  5,  4, T33, ref t, xbuf);
        SET_H(ref d, a, b, c,  8, 11, T34, ref t, xbuf);
        SET_H(ref c, d, a, b, 11, 16, T35, ref t, xbuf);
	    SET_H(ref b, c, d, a, 14, 23, T36, ref t, xbuf);
        SET_H(ref a, b, c, d,  1,  4, T37, ref t, xbuf);
        SET_H(ref d, a, b, c,  4, 11, T38, ref t, xbuf);
        SET_H(ref c, d, a, b,  7, 16, T39, ref t, xbuf);
        SET_H(ref b, c, d, a, 10, 23, T40, ref t, xbuf);
        SET_H(ref a, b, c, d, 13,  4, T41, ref t, xbuf);
        SET_H(ref d, a, b, c,  0, 11, T42, ref t, xbuf);
        SET_H(ref c, d, a, b,  3, 16, T43, ref t, xbuf);
        SET_H(ref b, c, d, a,  6, 23, T44, ref t, xbuf);
        SET_H(ref a, b, c, d,  9,  4, T45, ref t, xbuf);
        SET_H(ref d, a, b, c, 12, 11, T46, ref t, xbuf);
        SET_H(ref c, d, a, b, 15, 16, T47, ref t, xbuf);
        SET_H(ref b, c, d, a,  2, 23, T48, ref t, xbuf);

         /* Round 4. */
         /* Let [abcd k s t] denote the operation
              a = b + ((a + I(b,c,d) + X[k] + T[i]) <<< s). */
         /* Do the following 16 operations. */
        SET_I(ref a, b, c, d,  0,  6, T49, ref t, xbuf);
        SET_I(ref d, a, b, c,  7, 10, T50, ref t, xbuf);
        SET_I(ref c, d, a, b, 14, 15, T51, ref t, xbuf);
        SET_I(ref b, c, d, a,  5, 21, T52, ref t, xbuf);
        SET_I(ref a, b, c, d, 12,  6, T53, ref t, xbuf);
        SET_I(ref d, a, b, c,  3, 10, T54, ref t, xbuf);
        SET_I(ref c, d, a, b, 10, 15, T55, ref t, xbuf);
        SET_I(ref b, c, d, a,  1, 21, T56, ref t, xbuf);
        SET_I(ref a, b, c, d,  8,  6, T57, ref t, xbuf);
        SET_I(ref d, a, b, c, 15, 10, T58, ref t, xbuf);
        SET_I(ref c, d, a, b,  6, 15, T59, ref t, xbuf);
        SET_I(ref b, c, d, a, 13, 21, T60, ref t, xbuf);
        SET_I(ref a, b, c, d,  4,  6, T61, ref t, xbuf);
        SET_I(ref d, a, b, c, 11, 10, T62, ref t, xbuf);
        SET_I(ref c, d, a, b,  2, 15, T63, ref t, xbuf);
        SET_I(ref b, c, d, a,  9, 21, T64, ref t, xbuf);

         /* Then perform the following additions. (That is increment each
            of the four registers by the value it had before this block
            was started.) */
        ms.abcd[0] += a;
        ms.abcd[1] += b;
        ms.abcd[2] += c;
        ms.abcd[3] += d;
    }
}