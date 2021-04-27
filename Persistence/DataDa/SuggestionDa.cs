using HP.Data.Definitions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class SuggestionDa:IDisposable
    {
        private DataContext _context;
        public SuggestionDa(DataContext context)
        {
            _context = context;
        }
        public SuggestionDa()
        {
            _context = new DataContext();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void SaveSuggestion(Suggestion suggestion)
        {
            try
            {
                _context.Suggestions.Add(suggestion);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
