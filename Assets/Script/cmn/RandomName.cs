using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
public class RandomName  : MonoBehaviour{

	// Use this for initialization
	 // name array，需要上层设置（各列内容为：姓	男中字	男单字	男双字	女中字	女单字	女双字）
	
	public string[][] m_na;   
	
	  // 各种名字产生的过滤，需要上层设置（各列内容为：姓+单字 姓+双字 姓+中字+单字 姓+单字+单字） 
	
    public int[] m_prob;     
	
	// 性别偏移量（男生为1，女生为4） 
	
    public int m_genderOffset;       
    public System.Random m_rand;
   
    delegate string funcNameMaker();
    private funcNameMaker[] m_gen;
    public string funcNM_0()
    {
        string[]    n1  = m_na[m_genderOffset+1];
        int i0  = m_rand.Next() % m_na[0].Length;	
        int i1  = m_rand.Next() % n1.Length;
        return m_na[0][i0] + n1[i1];
    }
    public string funcNM_1()
    {
        string[] n1 = m_na[m_genderOffset + 2];
        int i0 = m_rand.Next() % m_na[0].Length;
        int i1 = m_rand.Next() % n1.Length;
        return m_na[0][i0] + n1[i1];
    }
    public string funcNM_2()
    {
        string[] n1 = m_na[m_genderOffset + 0];
        string[] n2 = m_na[m_genderOffset + 1];
        int i0 = m_rand.Next() % m_na[0].Length;
        int i1 = m_rand.Next() % n1.Length;
        int i2 = m_rand.Next() % n2.Length;
        return m_na[0][i0] + n1[i1] + n2[i2];
    }
    public string funcNM_3()
    {
        string[] n1 = m_na[m_genderOffset + 1];
        string[] n2 = m_na[m_genderOffset + 1];
        int i0 = m_rand.Next() % m_na[0].Length;
        int i1 = m_rand.Next() % n1.Length;
        int i2 = m_rand.Next() % n2.Length;
        return m_na[0][i0] + n1[i1] + n2[i2];
    }
   	public RandomName(string[][] _na, int[] _prob)
    {
        m_rand	= new	System.Random();

        m_na	= _na;
        // 根据prob生成函数对应的阀值数组 
        
        m_prob      = new int[4];
        m_prob[m_prob.Length-1] = 0;
        for (int i = m_prob.Length-2; i>=0; i--)
        {
            m_prob[i]   = m_prob[i+1] + _prob[i+1];
        }
		
        // 函数数组 
		
        m_gen       = new funcNameMaker[4];
        m_gen[0]    = funcNM_0;
        m_gen[1]    = funcNM_1;
        m_gen[2]    = funcNM_2;
        m_gen[3]    = funcNM_3;
    }
    // 根据性别生成一个名字（默认为男生）  
    public  string gen(bool isBoy)
    {
        if (isBoy)
        {
            m_genderOffset = 1;
        }
        else
        {
            m_genderOffset = 4;
        }
        // 根据概率确定生成函数 
        int idx = m_rand.Next() % 100;
        funcNameMaker funcGen=null;
        for (int i = 0; i < m_prob.Length; i++)
        {
            if (idx >= m_prob[i])
            {
                funcGen = m_gen[i];
                break;
            }
        }
        // 生成  
        return funcGen();
    }
}
