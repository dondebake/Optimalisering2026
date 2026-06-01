using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace ComponentAge.Dialogs.Design
{
    public class DesignTimeServices
    {
        public static string[] GetOpenedForms(IServiceProvider serviceProvider)
        {
            /*ISite site = ((IComponent)Instance).Site;
            if (site == null)
            {
                return new string[] { };
            }*/

            IDesignerEventService service = serviceProvider.GetService(typeof(IDesignerEventService)) as IDesignerEventService;
            IDesignerHost hostservice = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
            int idx = hostservice.RootComponentClassName.LastIndexOf('.');
            string namespaceName = "";
            if (idx > -1)
                namespaceName = hostservice.RootComponentClassName.Substring(0, idx);
            if (service == null)
            {
                return new string[] { };
            }

            ArrayList items = new ArrayList();
            foreach (IDesignerHost host in service.Designers)
            {
                if (host.RootComponentClassName == null || host.RootComponentClassName == "")
                    continue;

                int idx1 = host.RootComponentClassName.LastIndexOf('.');
                string namespaceName1 = host.RootComponentClassName.Substring(0, idx1);
                if (namespaceName != namespaceName1)
                    continue;

                IContainer container = host.Container;
                if (container != null)
                {
                    string formName = "";
                    for (int i = 0; i < container.Components.Count; i++)
                    {
                        IComponent comp = container.Components[i];
                        if (comp is Form && comp.Site != null)
                        {
                            formName = namespaceName + "." + comp.Site.Name;
                            if (items.IndexOf(formName) == -1)
                                items.Add(formName);
                            break;
                        }
                    }
                }
            }
            items.Sort();
            return (string[])items.ToArray(typeof(string));
        }
    }
}
