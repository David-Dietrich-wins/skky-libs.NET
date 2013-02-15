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
		public const string Const_Break = "<br />";
		public const string Const_Class = "class";
		public const string Const_NbSp = "&nbsp;";
		public const string Const_Strong = "strong";
		public const string Const_Label = "label";
		public const string Const_Style = "style";
		public const string Const_table = "table";
		public const string Const_td = "td";
		public const string Const_tr = "tr";

		public static string AddTagTD(string str)
		{
			if (string.IsNullOrEmpty(str))
				str = Const_NbSp;

			return XMLHelper.AddTag(Const_td, str);
		}
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
			return new LiteralControl(string.IsNullOrEmpty(text) ? Const_NbSp : text);
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
				label.Text = str + Const_Break;
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
			html.SetAttribute(Const_Class, className);
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

		public static string Base64EncodeString(string strUrl)
		{
			return Convert.ToBase64String(Encoding.ASCII.GetBytes(strUrl));
		}
		public static string Base64EncodeGuid(Guid guid)
		{
			return Base64EncodeString(guid.ToString());
		}

		public static string Base64DecodeString(string strUrl)
		{
			return Encoding.ASCII.GetString(Convert.FromBase64String(strUrl));
		}
		public static Guid Base64DecodeGuid(string strUrl)
		{
			return new Guid(Base64DecodeString(strUrl));
		}

		//** base64 decodes a utf16 string thats been converted to utf8, then base64 encoded.  .net only deals in utf8 when base64 enc/decoding
		public static string Utf16Base64Decode(string value)
		{
			var utf8value = Convert.FromBase64String(value);
			return Encoding.Unicode.GetString(utf8value);
		}

		public static string Utf16Base64Encode(string value)
		{
			var utf8value = Encoding.UTF8.GetBytes(value);
			return Convert.ToBase64String(utf8value);
		}

		public static string BuildHref(string text, string url)
		{
			return BuildHref(text, url, null);
		}
		public static string BuildHref(string text, string url, string target)
		{
			string str = string.Empty;
			if (!string.IsNullOrEmpty(url))
			{
				if (!url.Contains("//"))
					url = "http://" + url;

				str = "<a";
				str += XMLHelper.AddAttribute("href", url);
				if (target != null)
					str += XMLHelper.AddAttribute("target", string.IsNullOrEmpty(target) ? "_blank" : target);
				str += ">";
			}

			if (!string.IsNullOrEmpty(text))
			{
				str += text;
				str += "</a>";
			}

			return str;
		}
		public static string Href(string text, string url)
		{
			return Href(text, url, null);
		}
		public static string Href(string text, string url, string target)
		{
			string str = string.Empty;
			if (!string.IsNullOrEmpty(url))
			{
				//if (!url.Contains("//"))
				//	url = "http://" + url;

				str = "<a";
				str += XMLHelper.AddAttribute("href", url);
				if (target != null)
					str += XMLHelper.AddAttribute("target", string.IsNullOrEmpty(target) ? "_blank" : target);
				str += ">";
			}

			if (!string.IsNullOrEmpty(text))
			{
				str += text;
				str += "</a>";
			}

			return str;
		}
		public static string GetHREFImage(string alt, string url, string imagePath)
		{
			return GetHREFImage(alt, url, imagePath);
		}
		public static string GetHREFImage(string alt, string url, string imagePath, int width, int height)
		{
			string str = string.Empty;
			if (!string.IsNullOrEmpty(url))
			{
				if (!url.Contains("//"))
					url = "http://" + url;

				str = "<a";
				str += XMLHelper.AddAttribute("href", url);
				str += XMLHelper.AddAttribute("target", "_blank");
				str += ">";
			}

			str += BuildImg(imagePath, width, height, alt);

			if (!string.IsNullOrEmpty(url))
				str += "</a>";

			return str;
		}
		public static string BuildImg(string imagePath, int width, int height, string alt)
		{
			string str = string.Empty;
			if (string.IsNullOrEmpty(imagePath))
			{
				str = alt;
			}
			else
			{
				str += "<img";
				str += XMLHelper.AddAttribute("src", imagePath);
				if (height > 0)
					str += XMLHelper.AddAttribute("height", height.ToString());
				if (width > 0)
					str += XMLHelper.AddAttribute("width", width.ToString());
				str += XMLHelper.AddAttribute("alt", alt);
				str += XMLHelper.AddAttribute("border", "0");
				str += " />";
			}

			return str;
		}

		public static string Strong(string str)
		{
			return XMLHelper.AddTag(Const_Strong, str);
		}
		public static string Bold(string str)
		{
			return XMLHelper.AddTag("b", str);
		}
		public static string Label(string str, Color color)
		{
			return XMLHelper.AddTag(Const_Label, str, Const_Style, "background-color:" + color.ToHtmlString() + ";");
		}

		public static string PadRightNbsp(string s, int totalLength)
		{
			string str = s ?? string.Empty;
			int spacesToAdd = 1;
			if (str.Length < totalLength)
				spacesToAdd = totalLength - str.Length;

			for(int i = 0; i < spacesToAdd; ++i)
			{
				str += Const_NbSp;
			}

			return str;
		}

		public static string buildURL(string startURL, string file)
		{
			if (startURL == null)
				startURL = "";

			if (file == null)
				file = "";

			if (startURL.EndsWith("/") && file.StartsWith("/"))
				file = file.Mid(1);

			return startURL + file;
		}
	}
}
