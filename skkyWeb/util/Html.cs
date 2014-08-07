using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using skky.Types;
using skky.util;
using skky.web;
using System.Collections.Specialized;
using System.IO;

namespace skkyWeb.util
{
	public static class Html
	{
		#region WebControl implementations
		public static string GetRawHTML(Control ctl)
		{
			if (ctl == null)
				return string.Empty;

			StringBuilder sb = new StringBuilder();
			HtmlTextWriter tw = new HtmlTextWriter(new StringWriter(sb));

			ctl.RenderControl(tw);

			return sb.ToString();
		}

		public static ListItem GetSelectedItem(ListControl ctl)
		{
			if (ctl != null)
			{
				try
				{
					return ctl.SelectedItem;
				}
				catch (Exception ex)
				{
					skky.util.Trace.Warning("Error retrieving selected item from listbox.", ex);
				}
			}

			return null;
		}
		public static string GetSelectedName(ListControl ctl)
		{
			ListItem li = GetSelectedItem(ctl);
			if (li != null)
				return li.Text;

			return null;
		}
		public static string GetSelectedValue(ListControl ctl)
		{
			if (ctl != null)
			{
				try
				{
					if (ctl.SelectedIndex >= 0)
						return ctl.SelectedValue;
				}
				catch (Exception ex)
				{
					skky.util.Trace.Warning("Error retrieving selected value from dropdownlist.", ex);
				}
			}

			return string.Empty;
		}
		public static int GetSelectedValueInt(ListControl ctl)
		{
			return GetSelectedValue(ctl).ToInteger();
		}

		public static ListItem AddItemToListControl(ListControl ctl, StringString item, string selectedValue)
		{
			if (ctl != null && item != null)
			{
				ListItem li = new ListItem()
				{
					Text = item.stringValue,
					Value = item.string2Value
				};

				if (!string.IsNullOrEmpty(selectedValue) && !string.IsNullOrEmpty(item.string2Value) && selectedValue == item.string2Value)
					li.Selected = true;

				ctl.Items.Add(li);
				return li;
			}

			return null;
		}

		public static bool FillListControl(ListControl ctl, IEnumerable<string> list, string selectedValue, string firstItem)
		{
			bool selectedValueFound = false;
			if (ctl != null)
			{
				ctl.Items.Clear();

				if (!string.IsNullOrEmpty(firstItem))
				{
					ListItem li = AddItemToListControl(ctl, new StringString(firstItem, firstItem), firstItem);

					if (li != null && li.Selected == true)
						selectedValueFound = true;
				}

				if (list != null)
				{
					foreach (var cty in list)
					{
						ListItem li = AddItemToListControl(ctl, new StringString(cty, cty), selectedValueFound ? string.Empty : selectedValue);

						if (li != null && li.Selected == true)
							selectedValueFound = true;
					}
				}
			}

			return selectedValueFound;
		}

		public static bool FillListControl(ListControl ctl, IEnumerable<StringInt> list, StringInt firstItem)
		{
			return FillListControl(ctl, list, string.Empty, firstItem);
		}
		public static bool FillListControl(ListControl ctl, IEnumerable<StringInt> list, string selectedValue, StringInt firstItem)
		{
			bool selectedValueFound = false;
			if (ctl != null)
			{
				ctl.Items.Clear();

				if (firstItem != null)
				{
					ListItem li = AddItemToListControl(ctl, new StringString(firstItem.stringValue, firstItem.intValue.ToString()), selectedValue);

					if (li != null && li.Selected == true)
						selectedValueFound = true;
				}

				if (list != null)
				{
					foreach (var cty in list)
					{
						ListItem li = AddItemToListControl(ctl, new StringString(cty.stringValue, cty.intValue.ToString()), selectedValueFound ? string.Empty : selectedValue);

						if (li != null && li.Selected == true)
							selectedValueFound = true;
					}
				}
			}

			return selectedValueFound;
		}
		public static bool FillListControl(ListControl ctl, IEnumerable<StringString> list, StringString firstItem)
		{
			return FillListControl(ctl, list, string.Empty, firstItem);
		}
		public static bool FillListControl(ListControl ctl, IEnumerable<StringString> list, string selectedValue, StringString firstItem)
		{
			bool selectedValueFound = false;
			if (ctl != null)
			{
				ctl.Items.Clear();

				if (firstItem != null)
				{
					ListItem li = AddItemToListControl(ctl, firstItem, selectedValue);

					if (li != null && li.Selected == true)
						selectedValueFound = true;
				}

				if (list != null)
				{
					foreach (var cty in list)
					{
						ListItem li = AddItemToListControl(ctl, cty, selectedValueFound ? string.Empty : selectedValue);

						if (li != null && li.Selected == true)
							selectedValueFound = true;
					}
				}
			}

			return selectedValueFound;
		}

		public static LiteralControl GetLiteralControl(string text)
		{
			return new LiteralControl(string.IsNullOrEmpty(text) ? XMLHelper.CONST_NbSp : text);
		}
		public static Control AddChild(ControlCollection ctls, Control ctlToAdd)
		{
			if (ctls != null && ctlToAdd != null)
				ctls.Add(ctlToAdd);

			return ctlToAdd;
		}

		public static void AddLabelToControl(Control ctl, string str)
		{
			if (ctl != null)
				AddLabelToControl(ctl.Controls, str);
		}
		public static void AddLabelToControl(ControlCollection ctls, string str)
		{
			if (ctls != null && !string.IsNullOrEmpty(str))
			{
				System.Web.UI.WebControls.Label label = new System.Web.UI.WebControls.Label();
				label.Text = str + XMLHelper.CONST_Break;
				ctls.Add(label);
			}
		}

		public static void SetLabel(Label lbl, Color color, string str)
		{
			if (lbl != null)
			{
				lbl.Visible = true;
				lbl.ForeColor = color;
				lbl.Text = str ?? string.Empty;
			}
		}
		public static void SetInnerHtml(HtmlGenericControl ctl, Color color, string str)
		{
			if (ctl != null)
			{
				ctl.Visible = true;
				if (color == Color.Empty)
					color = Color.White;
				ctl.Style.Add(HtmlTextWriterStyle.BackgroundColor, color.ToHtmlString());
				ctl.InnerHtml = str ?? string.Empty;
			}
		}

		public static HtmlTableCell AddLiteralControl(this HtmlTableRow tr, string text)
		{
			return tr.AddChild(GetLiteralControl(text));
		}
		public static LiteralControl AddLiteralControl(this HtmlTableCell td, string text)
		{
			LiteralControl lc = GetLiteralControl(text);
			td.AddChild(lc);

			return lc;
		}
		public static Control AddChild(this Control ctl, Control ctlToAdd)
		{
			return AddChild(ctl.Controls, ctlToAdd);
		}
		public static Control SetChild(this Control ctl, Control ctlToAdd)
		{
			ctl.Controls.Clear();
			return ctl.AddChild(ctlToAdd);
		}
		public static HtmlTableCell AddChild(this HtmlTableRow tr, Control ctl)
		{
			if (ctl == null)
				return null;

			HtmlTableCell td = new HtmlTableCell();
			td.AddChild(ctl);

			tr.Cells.Add(td);

			return td;
		}

		public static void SetAttribute(this HtmlControl html, string attrName, string attrValue)
		{
			if (!string.IsNullOrEmpty(attrName) && !string.IsNullOrEmpty(attrValue))
				html.Attributes[attrName] = attrValue;
		}
		public static void SetClass(this HtmlControl html, string className)
		{
			html.SetAttribute(XMLHelper.CONST_Class, className);
		}

		public static HtmlTableRow AddTableRowIfValue(HtmlTable tbl, string name, int value)
		{
			if (value == 0)
				return null;

			return AddTableRow(tbl, name, value.ToString());
		}
		public static HtmlTableRow AddTableRowIfValue(HtmlTable tbl, string name, string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;

			return AddTableRow(tbl, name, value);
		}
		public static HtmlTableRow AddTableRow(HtmlTable tbl, string name, string value)
		{
			if (tbl == null)
				return null;

			HtmlTableRow tr = new HtmlTableRow();

			tr.AddLiteralControl(name + ": ");
			tr.AddLiteralControl(value);

			tbl.Rows.Add(tr);
			return tr;
		}
		#endregion
	}
}
