using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;

namespace Idera.SQLdm.DesktopClient.Controls.Designers
{
    /// <summary>
    /// Implements the control designer for editing the PropertiesControl at design time.
    /// </summary>
    public class PropertiesControlDesigner : ControlDesigner
    {
        private PropertiesControl propertiesControl;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            propertiesControl = component as PropertiesControl;

            // Hook up events
            ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            selectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
            changeService.ComponentRemoving += new ComponentEventHandler(OnComponentRemoving);
        }

        public override ICollection AssociatedComponents
        {
            get { return propertiesControl.Controls; }
        }

        public override DesignerVerbCollection Verbs
        {
            get
            {
                DesignerVerbCollection verbs = new DesignerVerbCollection();
                verbs.Add(new DesignerVerb("Add Page", new EventHandler(OnAddPage)));
                verbs.Add(new DesignerVerb("Delete Page", new EventHandler(OnDeleteSelectedPage)));
                verbs.Add(new DesignerVerb("Move Up", new EventHandler(OnMoveUp)));
                verbs.Add(new DesignerVerb("Move Down", new EventHandler(OnMoveDown)));
                return verbs;
            }
        }

        private void OnAddPage(object sender, EventArgs e)
        {
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            DesignerTransaction designerTransaction;
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // Add a property page to the collection
            designerTransaction = designerHost.CreateTransaction("Add Page");
            PropertyPage newPage = (PropertyPage)designerHost.CreateComponent(typeof(PropertyPage));
            changeService.OnComponentChanging(propertiesControl, null);
            propertiesControl.PropertyPages.Add(newPage);
            changeService.OnComponentChanged(propertiesControl, null, null, null);
            designerTransaction.Commit();
        }

        private void OnDeleteSelectedPage(object sender, EventArgs e)
        {
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            DesignerTransaction designerTransaction;
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // Delete the selected page
            designerTransaction = designerHost.CreateTransaction("Delete Selected Page");
            changeService.OnComponentChanging(propertiesControl, null);
            propertiesControl.DeleteSelectedPage();
            changeService.OnComponentChanged(propertiesControl, null, null, null);
            designerTransaction.Commit();
        }

        private void OnMoveUp(object sender, EventArgs e)
        {
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            DesignerTransaction designerTransaction;
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // Delete the selected page
            designerTransaction = designerHost.CreateTransaction("Move Up");
            changeService.OnComponentChanging(propertiesControl, null);
            propertiesControl.MoveSelectedPageUp();
            changeService.OnComponentChanged(propertiesControl, null, null, null);
            designerTransaction.Commit();
        }

        private void OnMoveDown(object sender, EventArgs e)
        {
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            DesignerTransaction designerTransaction;
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // Delete the selected page
            designerTransaction = designerHost.CreateTransaction("Move Down");
            changeService.OnComponentChanging(propertiesControl, null);
            propertiesControl.MoveSelectedPageDown();
            changeService.OnComponentChanged(propertiesControl, null, null, null);
            designerTransaction.Commit();
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            propertiesControl.DesignModeOnSelectionChanged();
        }

        private void OnComponentRemoving(object sender, ComponentEventArgs e)
        {
            PropertyPage page;
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));

            // If the user is removing a page
            if (e.Component is PropertyPage)
            {
                page = e.Component as PropertyPage;

                if (propertiesControl.PropertyPages.Contains(page))
                {
                    changeService.OnComponentChanging(propertiesControl, null);
                    propertiesControl.PropertyPages.Remove(page);
                    changeService.OnComponentChanged(propertiesControl, null, null, null);
                    return;
                }
            }

            // If the user is removing the control itself
            if (e.Component == propertiesControl)
            {
                for (int i = propertiesControl.PropertyPages.Count - 1; i >= 0; i--)
                {
                    page = propertiesControl.PropertyPages[i];
                    changeService.OnComponentChanging(propertiesControl, null);
                    propertiesControl.PropertyPages.Remove(page);
                    designerHost.DestroyComponent(page);
                    changeService.OnComponentChanged(propertiesControl, null, null, null);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // Unhook events
            selectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);
            changeService.ComponentRemoving -= new ComponentEventHandler(OnComponentRemoving);

            base.Dispose(disposing);
        }

        protected override bool GetHitTest(Point point)
        {
            if (propertiesControl.DesignModeHitTest(point.X, point.Y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
