using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

internal class MenuContainer : IContainer
{
    public ComponentCollection GetComponents()
    {
        throw new NotImplementedException();
    }

    private ArrayList tsMenuItemList;


    public MenuContainer()
    {
        tsMenuItemList= new ArrayList();
    }

    public void Add(IComponent component)
    {
        tsMenuItemList.Add(component);
    }

    public void Add(IComponent component, string name)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Remove(IComponent component)
    {
        for (int i = 0; i < tsMenuItemList.Count; ++i)
        {
            if (component.Equals(tsMenuItemList[i]))
            {
                tsMenuItemList.RemoveAt(i);
                break;
            }
        }
    }

    public ComponentCollection Components
    {
        get
        {
            IComponent[] datalist = new ToolStripMenuItem[tsMenuItemList.Count];
           tsMenuItemList.CopyTo(datalist);
            return new ComponentCollection(datalist);
        }
    }
}

