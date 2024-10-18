using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultDataHandler
{
    static private ResultDataHandler instance;
    public static ResultDataHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ResultDataHandler(null);
            }
            return instance;
        }
    }

    public ResultDataHandler(ResultData result)
    {
        this.result = result;
    }



    public ResultData result;
}
