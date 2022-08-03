using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.Util.Media;

namespace GammaJul.ReSharper.EnhancedTooltip.VisualStudio {
	internal enum XmlDocListType {
		TABLE,
		BULLET,
		NUMBER,
	}
	public class XmlDocRichTextPresenterEx : XmlDocVisitor {
		private static readonly TextStyle ourCodeStyle = TextStyle.FromForeColor(Color.DarkBlue.ToJetRgbaColor());
		private static readonly TextStyle ourCRefStyle = new TextStyle(JetFontStyles.Bold);
		private static readonly TextStyle ourNormalStyle = TextStyle.Default;
		private readonly PsiLanguageType myLanguageType;
		[CanBeNull]
		private readonly DeclaredElementPresenterTextStyles myTextStyles;
		private readonly IPsiModule myModule;
		private readonly List<JetBrains.UI.RichText.RichText> myStrings = new List<JetBrains.UI.RichText.RichText>();
		private JetBrains.UI.RichText.RichText myLastString = null!;
		private int myLevel;

		protected XmlDocRichTextPresenterEx(XmlNode node, PsiLanguageType languageType, DeclaredElementPresenterTextStyles textStyles, IPsiModule module) {
			this.myModule = module;
			this.myLanguageType = languageType;
			this.myTextStyles = textStyles;
			this.NewLine();
			this.VisitNode(node);
		}

		[NotNull]
		protected RichTextBlock RichTextBlock {
			get {
				while (this.myStrings.Count > 0 && string.IsNullOrWhiteSpace(this.myStrings[0].Text))
					this.myStrings.RemoveAt(0);
				while (this.myStrings.Count > 0 && string.IsNullOrWhiteSpace(this.myStrings.Last<JetBrains.UI.RichText.RichText>().Text))
					this.myStrings.RemoveAt(this.myStrings.Count - 1);
				RichTextBlock richTextBlock = new RichTextBlock(new RichTextBlockParameters());
				foreach (JetBrains.UI.RichText.RichText line in this.myStrings)
					richTextBlock.Add(line);
				return richTextBlock;
			}
		}

		public override void VisitExample(XmlElement element) {
		}

		public override void VisitException(XmlElement element) {
		}

		public override void VisitInclude(XmlElement element) {
		}

		public override void VisitPermission(XmlElement element) {
		}

		public override void VisitReturns(XmlElement element) {
		}

		public override void VisitParam(XmlElement element) {
		}

		public override void VisitTypeParam(XmlElement element) {
		}

		public override void VisitElement(XmlElement element) {
			++this.myLevel;
			base.VisitElement(element);
			--this.myLevel;
		}

		public override void VisitValue(XmlElement element) {
			if (!(element.PreviousSibling is XmlText) && !(element.NextSibling is XmlText))
				return;
			base.VisitValue(element);
		}

		public override void VisitText(XmlText text) {
			this.Append(XmlDocRichTextPresenterEx.RemoveExtraSpaces(text.InnerText), XmlDocRichTextPresenterEx.ourNormalStyle);
			base.VisitText(text);
		}

		public override void VisitCData(XmlCDataSection cdataSection) {
			this.Append(XmlDocRichTextPresenterEx.RemoveExtraSpaces(cdataSection.InnerText), XmlDocRichTextPresenterEx.ourNormalStyle);
			base.VisitCData(cdataSection);
		}

		public override void VisitWhitespace(XmlWhitespace whitespace) {
			if (this.myLevel > 0)
				this.Append(" ", XmlDocRichTextPresenterEx.ourNormalStyle);
			base.VisitWhitespace(whitespace);
		}

		public override void VisitSignificantWhitespace(XmlSignificantWhitespace significantWhitespace) {
			this.Append(significantWhitespace.InnerText, XmlDocRichTextPresenterEx.ourNormalStyle);
			base.VisitSignificantWhitespace(significantWhitespace);
		}

		public override void VisitUnknownTag(XmlElement element) {
			string localName = element.LocalName;
			if (localName.Equals("filterpriority", StringComparison.OrdinalIgnoreCase))
				return;
			if (localName.Equals("br", StringComparison.OrdinalIgnoreCase))
				this.Append("\n");
			else
				base.VisitUnknownTag(element);
		}

		public override void VisitCommonSee(XmlElement element) {
			XmlAttribute attribute1 = element.Attributes["cref"] ?? element.Attributes["href"] ?? element.Attributes["typeparamref"] ?? element.Attributes["paramref"];
			if (attribute1 != null)
				this.Append(this.ProcessCRef(attribute1.Value));
			XmlAttribute attribute2 = element.Attributes["langword"];
			if (attribute2 != null)
				this.Append(attribute2.Value, XmlDocRichTextPresenterEx.ourCRefStyle);
			XmlAttribute attribute3 = element.Attributes["title"];
			if (attribute3 == null)
				return;
			this.Append(attribute3.Value, XmlDocRichTextPresenterEx.ourCRefStyle);
		}

		private void WrapInNewLine(Action<XmlElement> action, XmlElement element) {
			this.Append("\n");
			action(element);
			this.Append("\n");
		}

		public override void VisitPara(XmlElement element) => this.WrapInNewLine(new Action<XmlElement>(base.VisitPara), element);

		public override void VisitSummary(XmlElement element) => this.WrapInNewLine(new Action<XmlElement>(base.VisitSummary), element);

		public override void VisitRemarks(XmlElement element) => this.WrapInNewLine(new Action<XmlElement>(base.VisitRemarks), element);

		public override void VisitC(XmlElement element) => this.Append(element.InnerText, XmlDocRichTextPresenterEx.ourCodeStyle);

		public override void VisitCode(XmlElement element) {
			if (!element.InnerText.ContainsNewLine()) {
				this.VisitC(element);
			} else {
				string[] strArray = element.InnerText.SplitByNewLine();
				if (strArray.Length == 0 || strArray[0] != "")
					this.Append("\n");
				for (int index = 0; index < strArray.Length; ++index) {
					string str = strArray[index];
					this.Append(str == "" ? " " : str, XmlDocRichTextPresenterEx.ourCodeStyle);
					if (index < strArray.Length - 1)
						this.Append("\n");
				}
				if (strArray.Length == 0 || !(strArray[strArray.Length - 1] != string.Empty))
					return;
				this.Append("\n");
			}
		}

		public override void VisitList(XmlElement element) {
			XmlDocListType xmlDocListType = XmlDocListType.TABLE;
			XmlAttribute attribute = element.Attributes["type"];
			if (attribute != null) {
				if (attribute.Value == "bullet")
					xmlDocListType = XmlDocListType.BULLET;
				if (attribute.Value == "number")
					xmlDocListType = XmlDocListType.NUMBER;
			}
			XmlNode node1 = element.SelectSingleNode("listheader");
			XmlNodeList xmlNodeList = element.SelectNodes("item");
			int val1 = XmlDocRichTextPresenterEx.GetListItemTermSize(node1);
			foreach (XmlNode node2 in xmlNodeList)
				val1 = Math.Max(val1, XmlDocRichTextPresenterEx.GetListItemTermSize(node2));
			int num1 = 0;
			switch (xmlDocListType) {
				case XmlDocListType.BULLET:
					num1 = 1;
					break;
				case XmlDocListType.NUMBER:
					num1 = (int)Math.Floor(Math.Log10((double)xmlNodeList.Count)) + 1;
					break;
			}
			this.Append("\n");
			if (node1 != null) {
				if (num1 > 0)
					this.Append(new string(' ', num1 + 1));
				XmlNode xmlNode = node1.SelectSingleNode("term");
				string str = xmlNode == null ? "" : xmlNode.InnerText.Trim();
				this.Append(str, XmlDocRichTextPresenterEx.ourNormalStyle);
				this.Append(new string(' ', val1 - str.Length + 1), XmlDocRichTextPresenterEx.ourNormalStyle);
				XmlNode node3 = node1.SelectSingleNode("description");
				if (node3 != null)
					this.VisitNode(node3);
				this.Append("\n");
			}
			int num2 = 1;
			foreach (XmlNode xmlNode1 in xmlNodeList) {
				switch (xmlDocListType) {
					case XmlDocListType.BULLET:
						this.Append("- ");
						break;
					case XmlDocListType.NUMBER:
						string str1 = num2.ToString((IFormatProvider)CultureInfo.InvariantCulture);
						this.Append(str1, XmlDocRichTextPresenterEx.ourNormalStyle);
						this.Append(new string(' ', num1 - str1.Length + 1), XmlDocRichTextPresenterEx.ourNormalStyle);
						break;
				}
				++num2;
				XmlNode xmlNode2 = xmlNode1.SelectSingleNode("term");
				if (xmlNode2 == null) {
					this.Append(new string(' ', val1 + 1), XmlDocRichTextPresenterEx.ourNormalStyle);
				} else {
					string str2 = xmlNode2.InnerText.Trim();
					this.Append(str2, XmlDocRichTextPresenterEx.ourNormalStyle);
					this.Append(new string(' ', val1 - str2.Length + 1), XmlDocRichTextPresenterEx.ourNormalStyle);
				}
				XmlNode node4 = xmlNode1.SelectSingleNode("description");
				if (node4 != null)
					this.VisitNode(node4);
				this.Append("\n");
			}
		}

		private void ProcessRefTagNameAttr(XmlElement element) {
			string attribute = element.GetAttribute("name");
			if (string.IsNullOrEmpty(attribute))
				return;
			this.Append(attribute, XmlDocRichTextPresenterEx.ourCRefStyle);
		}

		public override void VisitParamRef(XmlElement element) => this.ProcessRefTagNameAttr(element);

		public override void VisitTypeParamRef(XmlElement element) => this.ProcessRefTagNameAttr(element);

		private JetBrains.UI.RichText.RichText ProcessCRef(string value) {
			DeclaredElementPresenterStyle presenterStyle = new DeclaredElementPresenterStyle() {
				ShowAccessRights = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowAccessRights,
				ShowModifiers = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowModifiers,
				ShowParametersForDelegates = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowParametersForDelegates,
				ShowEntityKind = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowEntityKind,
				ShowName = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowName,
				ShowExplicitInterfaceQualification = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowExplicitInterfaceQualification,
				ShowNameInQuotes = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowNameInQuotes,
				ShowType = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowType,
				ShowParameterTypes = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowParameterTypes,
				ShowParameterNames = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowParameterNames,
				ShowTypesQualified = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowTypesQualified,
				ShowMemberContainer = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowMemberContainer,
				ShowTypeContainer = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowTypeContainer,
				ShowNamespaceContainer = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowNamespaceContainer,
				ShowParameterContainer = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowParameterContainer,
				ShowConstantValue = XmlDocPresenterUtil.LinkedElementPresentationStyle.ShowConstantValue,
				TextStyles = this.myTextStyles ?? XmlDocPresenterUtil.LinkedElementPresentationStyle.TextStyles
			};

			return XmlDocRichTextPresenterEx.ProcessCRef(value, this.myLanguageType, this.myModule, presenterStyle);
		}

		public static JetBrains.UI.RichText.RichText ProcessCRef(string crefValue, PsiLanguageType languageType, IPsiModule psiModule, DeclaredElementPresenterStyle presenterStyle) {
			var element = psiModule == null ? null : XMLDocUtil.ResolveId(psiModule.GetPsiServices(), crefValue, psiModule, true);
			return element == null || !element.IsValid() ? new JetBrains.UI.RichText.RichText(XmlDocPresenterUtil.ProcessCref(crefValue), XmlDocRichTextPresenterEx.ourCRefStyle) : DeclaredElementPresenter.Format(languageType, presenterStyle, element);
		}

		private static int GetListItemTermSize(XmlNode node) {
			if (node == null)
				return 0;
			XmlNode xmlNode = node.SelectSingleNode("term");
			return xmlNode == null ? 0 : xmlNode.InnerText.Trim().Length;
		}

		protected void NewLine() {
			this.myLastString = new JetBrains.UI.RichText.RichText();
			this.myStrings.Add(this.myLastString);
		}

		protected void Append(JetBrains.UI.RichText.RichText richText) => this.myLastString.Append(richText);

		protected void Append(string str, TextStyle style) {
			bool flag = false;
			if (str.EndsWith("\n")) {
				str = str.Substring(0, str.Length - 1);
				flag = true;
			}
			if (str != "") {
				if (this.myLastString.Text == "") {
					this.myLastString.Append(str, style);
				} else {
					int c1 = (int)this.myLastString.Text[this.myLastString.Text.Length - 1];
					char c2 = str[0];
					if (!char.IsWhiteSpace((char)c1)) {
						if (c2 == '\n') {
							this.NewLine();
							str = str.Substring(1);
						}
						this.myLastString.Append(str, style);
					} else {
						if (c2 == '\n')
							this.NewLine();
						if (char.IsWhiteSpace(c2))
							str = str.Substring(1);
						this.myLastString.Append(str, style);
					}
				}
			}
			if (!flag || !(this.myLastString.Text != ""))
				return;
			this.NewLine();
		}

		protected void Append(string str) => this.Append(str, TextStyle.Default);

		protected static string RemoveExtraSpaces(string str) {
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (char c in str) {
				if (char.IsWhiteSpace(c)) {
					if (!flag)
						stringBuilder.Append(' ');
					flag = true;
				} else {
					stringBuilder.Append(c);
					flag = false;
				}
			}
			return stringBuilder.ToString();
		}

		[NotNull]
		public static RichTextBlock Run(XmlNode node, bool includeHeader, PsiLanguageType languageType, DeclaredElementPresenterTextStyles textStyles, IPsiModule module) {
			return node == null ? new RichTextBlock(new RichTextBlockParameters()) : new XmlDocRichTextPresenterEx(node, languageType, textStyles, module).RichTextBlock;
		}
	}
}
