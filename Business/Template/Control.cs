using System;

namespace Wayne.Payment.Products.iXConfigurator.Template
{
    public abstract class Control : Element
    {
        protected Control(control control)
            : base(control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            Enabled = control.enabled ?? string.Empty;
        }

        protected Control(string visible, string enabled)
            : base(visible)
        {
            if (string.IsNullOrWhiteSpace(enabled))
            {
                throw new ArgumentNullException(nameof(enabled));
            }

            Enabled = enabled;
        }

        public abstract control ToControl();

        public string Enabled { get; set; }

        public static Control CreateControl(control control)
        {
            Control newControl;

            if (control is radio)
            {
                newControl = new RadioControl((radio) control);
            }
            else if (control is check)
            {
                newControl = new CheckControl((check) control);
            }
            else if (control is combo)
            {
                newControl = new ComboControl((combo)control);
            }
            else if (control is list)
            {
                newControl = new ListControl((list)control);
            }
            else if (control is table)
            {
                newControl = new TableControl((table) control);
            }
            else if (control is text)
            {
                newControl = new TextControl((text) control);
            }
            else
            {
                throw new ArgumentOutOfRangeException(control.GetType().AssemblyQualifiedName);
            }

            return newControl;
        }
    }
}
