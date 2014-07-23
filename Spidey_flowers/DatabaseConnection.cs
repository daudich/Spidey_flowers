using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient; 

namespace Spidey_flowers
{
	private string sql_string;
	private string strCon;

	public string Sql{
	    set{ sql_string = value; }
	}

    public string connectionString{
	    set{ strCon = value; }
	}

}
