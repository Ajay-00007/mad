using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace StudentManagementSystem.TagHelpers
{
    [HtmlTargetElement("form-group")]
    public class FormGroupTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; } = null!;

        [HtmlAttributeName("label-class")]
        public string LabelClass { get; set; } = "form-label";

        [HtmlAttributeName("input-class")]
        public string InputClass { get; set; } = "form-control";

        [HtmlAttributeName("type")]
        public string InputType { get; set; } = "text";

        [HtmlAttributeName("placeholder")]
        public string? Placeholder { get; set; }

        [HtmlAttributeName("helper-text")]
        public string? HelperText { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "mb-3");

            var label = $@"<label class=""{LabelClass}"" for=""{For.Name}"">{For.Metadata.DisplayName ?? For.Name}</label>";
            
            var inputAttributes = $@"class=""{InputClass}"" type=""{InputType}"" id=""{For.Name}"" name=""{For.Name}""";
            if (Placeholder != null)
            {
                inputAttributes += $@" placeholder=""{HtmlEncoder.Default.Encode(Placeholder)}""";
            }
            if (For.Metadata.IsRequired)
            {
                inputAttributes += " required";
            }

            string input;
            if (InputType == "textarea")
            {
                input = $@"<textarea {inputAttributes}></textarea>";
            }
            else
            {
                input = $@"<input {inputAttributes} value=""{For.Model}"" />";
            }

            var validation = $@"<span class=""invalid-feedback"" data-valmsg-for=""{For.Name}"" data-valmsg-replace=""true""></span>";

            var helperTextHtml = string.Empty;
            if (!string.IsNullOrEmpty(HelperText))
            {
                helperTextHtml = $@"<small class=""form-text text-muted"">{HelperText}</small>";
            }

            output.Content.SetHtmlContent($"{label}{input}{validation}{helperTextHtml}");
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }

    [HtmlTargetElement("form-group-select")]
    public class FormGroupSelectTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; } = null!;

        [HtmlAttributeName("asp-items")]
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? Items { get; set; }

        [HtmlAttributeName("label-class")]
        public string LabelClass { get; set; } = "form-label";

        [HtmlAttributeName("select-class")]
        public string SelectClass { get; set; } = "form-select";

        [HtmlAttributeName("helper-text")]
        public string? HelperText { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "mb-3");

            var label = $@"<label class=""{LabelClass}"" for=""{For.Name}"">{For.Metadata.DisplayName ?? For.Name}</label>";

            var selectStart = $@"<select class=""{SelectClass}"" id=""{For.Name}"" name=""{For.Name}""";
            if (For.Metadata.IsRequired)
            {
                selectStart += " required";
            }
            selectStart += ">";

            var options = string.Empty;
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    var selected = item.Value == For.Model?.ToString() ? " selected" : "";
                    options += $@"<option value=""{item.Value}""{selected}>{item.Text}</option>";
                }
            }

            var selectEnd = "</select>";

            var validation = $@"<span class=""invalid-feedback"" data-valmsg-for=""{For.Name}"" data-valmsg-replace=""true""></span>";

            var helperTextHtml = string.Empty;
            if (!string.IsNullOrEmpty(HelperText))
            {
                helperTextHtml = $@"<small class=""form-text text-muted"">{HelperText}</small>";
            }

            output.Content.SetHtmlContent($"{label}{selectStart}{options}{selectEnd}{validation}{helperTextHtml}");
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}