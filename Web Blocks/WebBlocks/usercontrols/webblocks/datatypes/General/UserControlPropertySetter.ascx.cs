/****
    Mentor Web Blocks for Umbraco
    Copyright (C) 2013 Mentor Digital 
    (Mentor Communications Consultancy Ltd, 4 West End, Somerset Street, Bristol, BS2 8NE)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
****/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using umbraco.cms.businesslogic.macro;
using umbraco.cms.businesslogic.web;
using WebBlocks.BusinessLogic.Helpers;
using WebBlocks.BusinessLogic.Model;

namespace WebBlocks.usercontrols.webblocks.datatypes
{
    public partial class UserControlPropertySetter : UserControl, umbraco.editorControls.userControlGrapper.IUsercontrolDataEditor
    {
        protected List<UserControlProperty> userControlProperties = new List<UserControlProperty>();
        protected Macro currentMacro; 
        public object value {
            get { return userControlProperties.ToJson(); }
            set 
            { 
                string json = (string) value;
                if (json != null && json != "")
                {
                    userControlProperties = json.FromJson<List<UserControlProperty>>();
                }
            }
        }

        

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            int id = Convert.ToInt32(Request.QueryString["id"]);
            Document doc = new Document(id);
            string alias = doc.ContentType.Alias;

            currentMacro = GetUserControlMacro(alias);

            if (currentMacro != null)
            {
                UserControl userControl = (UserControl)this.LoadControl("~/" + currentMacro.Type);
                Type userControlType = userControl.GetType().UnderlyingSystemType;

                foreach (var property in currentMacro.Properties)
                {
                    plcParameters.Controls.Add(CreateEditorForField(property, userControlType));
                }
            }
            else
            {
                plcParameters.Controls.Add(new Label() { 
                    ID = "lblError",
                    Text = string.Format("A macro doesn't exist with the alias '{0}'", alias)
                });
            }
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //for the first load only set the values
                SetEditorValues();
            }
            else
            {
                //build up the usercontrol properties to save for this block
                BuildUserControlProperties();
            }
        }

        /// <summary>
        /// Gets usercontrol macro based on alias
        /// </summary>
        /// <returns>A usercontrol macro if the macro is found and is a usercontrol macro</returns>
        protected Macro GetUserControlMacro(string alias)
        {
            IEnumerable<Macro> macros = Macro.GetAll();

            return macros.FirstOrDefault(m => !string.IsNullOrEmpty(m.Type) && m.Alias == alias);
        }

        /// <summary>
        /// Creates a textbox to allow editing
        /// </summary>
        /// <returns>A control to add to the container</returns>
        protected Control CreateEditorForField(MacroProperty property, Type userControlType)
        {
            HtmlGenericControl div = new HtmlGenericControl("div");
            div.Attributes.Add("class", "wbPropertyRow");
            Label label = new Label();
            WebControl editorControl = GetCorrectControl(property.Alias, userControlType);

            label.ID = string.Format("lbl{0}", property.Alias);
            label.Text = string.Format("{0}: ", property.Name);

            editorControl.ID = string.Format("txt{0}", property.Alias);
            editorControl.Attributes.Add("propertytype", property.Type.Alias);

            div.Controls.Add(label);
            div.Controls.Add(editorControl);

            return div;
        }

        /// <summary>
        /// Set the text box values from what is saved
        /// </summary>
        protected void SetEditorValues()
        {
            foreach (MacroProperty property in currentMacro.Properties)
            {
                WebControl control = (WebControl)plcParameters.FindControl(string.Format("txt{0}", property.Alias));

                UserControlProperty storedProperty = userControlProperties.FirstOrDefault(p => p.Name == property.Alias);
                if (storedProperty != null)
                {
                    SetControlValue(storedProperty.Value, control);
                }
            }
        }

        /// <summary>
        /// Builds a list of UserControlProperty with the settings that match the data entered for each property
        /// </summary>
        /// <returns></returns>
        protected void BuildUserControlProperties()
        {
            userControlProperties.Clear();
            foreach (MacroProperty property in currentMacro.Properties)
            {
                WebControl control = (WebControl)plcParameters.FindControl(string.Format("txt{0}", property.Alias));

                userControlProperties.Add(new UserControlProperty()
                {
                    Name = property.Alias,
                    Type = property.Type.Alias,
                    Value = GetControlValue(control)
                });
            }
        }

        #region Get and Set Control Values (based on control types in the GetCorrectControl method)
        protected string GetControlValue(WebControl control)
        {
            //dictionary to get the correct type
            Dictionary<Type, Func<WebControl, string>> relatedControls = new Dictionary<Type, Func<WebControl, string>>()
                {
                    //the type          will use calendar control
                    { typeof(Calendar), (c) => ((Calendar)c).SelectedDate.ToString() },
                    { typeof(TextBox), (c) => ((TextBox)c).Text },
                    { typeof(CheckBox), (c) => ((CheckBox)c).Checked.ToString() },
                    //e.g. { typeof(YourCustomControl), (type) => new YourCustomControl() } }
                };
            Type controlType = control.GetType();
            return relatedControls[controlType](control);
        }

        protected void SetControlValue(string value, WebControl control)
        {
            //dictionary to set the correct type value
            Dictionary<Type, Action<string, WebControl>> relatedControls = new Dictionary<Type, Action<string, WebControl>>()
                {
                    //the type          set the control
                    { typeof(Calendar), (v, c) => ((Calendar)c).SelectedDate = DateTime.Parse(v) },
                    { typeof(TextBox), (v, c) => ((TextBox)c).Text = value },
                    { typeof(CheckBox), (v, c) =>
                        {
                            //in case the property wasn't a bool on previous load, parse it
                            bool defaultBool = false;
                            if (!bool.TryParse(v, out defaultBool))
                                defaultBool = false;
                            ((CheckBox)c).Checked = defaultBool;
                        } },
                    //e.g. { typeof(YourCustomControl), (type) => new YourCustomControl() } }
                };

            //set the value
            relatedControls[control.GetType().UnderlyingSystemType](value, control);
        }
        #endregion

        /// <summary>
        /// Gets the correct control to display and edit the property
        /// </summary>
        /// <param name="propertyName">The alias of the property</param>
        /// <param name="userControlType">the type of usercontrol</param>
        /// <returns>A web control to allow editing of the property</returns>
        protected WebControl GetCorrectControl(string propertyName, Type userControlType)
        {
            WebControl control = null;
            Type propertyType = GetPropertyType(propertyName, userControlType);
            //to use specific controls, add to this function
            //also update GetControlValue to get the value if you use that control
            Dictionary<Type, Func<WebControl>> relatedControls = new Dictionary<Type, Func<WebControl>>()
                {
                    //the type          will use calendar control
                    { typeof(DateTime), () => new Calendar() },
                    { typeof(TimeSpan), () => new TextBox() },
                    { typeof(bool), () => new CheckBox() }
                    //e.g. { typeof(YourCustomControl), (type) => new YourCustomControl() } }
                };

            Func<WebControl> relatedControlFunc = null;

            if (relatedControls.ContainsKey(propertyType))
                relatedControlFunc = relatedControls[propertyType];

            //if a related function is found
            if (relatedControlFunc != null)
            {
                //call the function to get the related control
                control = relatedControlFunc();
            }
            else
            {
                //otherwise as a fallback use a textbox
                control = new TextBox();
            }

            return control;
        }

        /// <summary>
        /// Gets the property type from a given property name and given the type of usercontrol
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="userControlType">the type of usercontrol</param>
        /// <returns>The type of the property (string etc)</returns>
        protected Type GetPropertyType(string propertyName, Type userControlType)
        {
            Type propertyType = null;
            PropertyInfo propertyInfo = userControlType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                propertyType = propertyInfo.PropertyType;
            }
            return propertyType ?? typeof(string);
        }
        
    }
}