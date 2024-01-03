using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
public class RandomNameEn  : MonoBehaviour{

	public string[][] m_na;   
	
    public int m_genderOffset;       
    public System.Random m_rand;

   	public RandomNameEn(string[][] _na)
    {
        m_rand	= new	System.Random();

        m_na	= _na;
    }
    // 根据性别生成一个名字（默认为男生）  
    public  string gen(bool isBoy, bool isRandSex)
    {
		if (isRandSex)
		{
			isBoy			= (m_rand.Next()%2)==0?true:false;
		}
		int		pos			= m_rand.Next()%m_na[0].Length;
		string	lastName	= m_na[0][pos];
		string	firstName	= "";
        if (isBoy)
        {
			pos				= m_rand.Next()%m_na[1].Length;
            firstName		= m_na[1][pos];
        }
        else
        {
            pos				= m_rand.Next()%m_na[2].Length;
            firstName		= m_na[2][pos];
        }
		
		return firstName + "." + lastName;
    }
}

