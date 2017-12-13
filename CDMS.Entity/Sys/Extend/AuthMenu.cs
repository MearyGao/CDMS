using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDMS.Entity
{
    public class AuthMenu
    {
        public IEnumerable<Menu> Menus { get; set; }

        public IEnumerable<MenuColumn> Columns { get; set; }

        public AuthMenu() { }

        public AuthMenu(IEnumerable<Menu> ms)
        {
            this.Menus = ms;
        }

        public AuthMenu(IEnumerable<Menu> ms, IEnumerable<MenuColumn> cs)
        {
            this.Menus = ms;
            this.Columns = cs;
        }

        public bool HaveMenuRight()
        {
            var ms = this.Menus;
            return ms != null && ms.Count() > 0;
        }
    }
}
