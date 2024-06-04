using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoryDb
{
    //the reaosn fir this intreface as well as IColumn is just becuase c# donee'ss have teh feature of wildcard generics and hahving it evaluated at turnitme without boxing (synmiac uses boxing), so i cant do smething like Dicitnoary<string, Column<?>> rather i have to do DIcitnoary<string, IColumn>, the former of whch would allow me to not need GetCell(int rowIndex, out dynamic val), but i tried to  minzime it a smuch as i could by having methods be neric eben theough the interface isn't and havign the tye inferred fom context, and moving a much of the work wiht th especiifc type ot eb on the level of the Column which knws what type it is dealing with.
    public interface IColumnWrapper
    {
    }
}
