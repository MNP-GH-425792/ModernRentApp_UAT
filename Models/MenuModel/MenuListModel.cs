namespace RENT_MVC_PROJECT.Models.MenuModel
{
    public class MenuListModel
    {
        //public string MENU_NAME { get; set; }=string.Empty;
        public List<MenuNameModel> M_NAME { get; set; }
        public List<SubMenuNameModel> S_NAME { get; set; }
        public List<ChildMenuNameModel> C_NAME { get; set; }
        public string idcheck { get; set; }

        public string EmpCode { get; set; } = string.Empty;
        public string EMP_NAME { get; set; } = string.Empty;
        public string Response {get;set;}= string.Empty;
        public string Response1 { get; set; } = string.Empty;
    }

    public class MenuNameModel

    {
        public string MENU_NAME { get; set; }
        public string MENU_ID { get; set; }



    }

    public class SubMenuNameModel

    {
        public string SUBMENU_NAME { get; set; }
        public string MAIN_MENU_ID { get; set; }
        public string LINK { get; set; }
        public string SUBMENU_ID { get; set; }
        public string SUBMENUDIVIDER_ID { get; set; }

        
    }


    public class ChildMenuNameModel
    {
        public string CHILDMENU_NAME { get; set; }
        public string SUBMENU_ID { get; set; }
        public string LINK { get; set; }
 
    }
}
