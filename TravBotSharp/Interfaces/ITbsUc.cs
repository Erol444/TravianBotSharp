using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravBotSharp.Interfaces
{
    /// <summary>
    /// Interface for TBS UserControls
    /// </summary>
    public interface ITbsUc
    {
        /// <summary>
        /// Initialize the view
        /// </summary>
        /// <param name="obj"></param>
        void Init(object obj);
        /// <summary>
        /// Update the view
        /// </summary>
        void UpdateUc();
        
    }
}
