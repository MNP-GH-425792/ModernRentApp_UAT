using RENT_MVC_PROJECT.Models.MenuModel;

namespace AUDIT_MVC_Project.MenuModel
{
    public class MenuListModel1
    {
        //public string MENU_NAME { get; set; }=string.Empty;
        public List<MenuNameModel> M_NAME { get; set; }
       
        public List<SubMenuNameModel> S_NAME { get; set; }
        public string idcheck { get; set; }

    }

    public class MenuNameModel1

    {
        public string MENU_NAME { get; set; }
        public string MENU_ID { get; set; }
      

    }

    public class SubMenuNameModel1

    {
         public string SUBMENU_NAME { get; set; }
        //public string SUB_MENU { get; set; }
        
        public string MAIN_MENU_ID { get; set; }
        public string LINK { get; set; }
    }
}
