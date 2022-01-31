using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DirectBot.API.TagHalpers;

[HtmlTargetElement("input", Attributes = PlaceholderAttributeName, TagStructure = TagStructure.WithoutEndTag)]
public class InputPlaceholderTagHelper : InputTagHelper
{
    private const string PlaceholderAttributeName = "asp-placeholder-for";

    public InputPlaceholderTagHelper(IHtmlGenerator generator) : base(generator)
    {
    }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName(PlaceholderAttributeName)]
    public ModelExpression Placeholder { get; set; } = null!;


    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);

        var placeholder = GetPlaceholder(Placeholder.ModelExplorer);

        if (!output.Attributes.TryGetAttribute("placeholder", out _))
        {
            output.Attributes.Add(new TagHelperAttribute("placeholder", placeholder));
        }
    }

    private string GetPlaceholder(ModelExplorer modelExplorer)
    {
        var placeholder = modelExplorer.Metadata.Placeholder;

        if (String.IsNullOrWhiteSpace(placeholder))
            placeholder = modelExplorer.Metadata.GetDisplayName();

        return placeholder;
    }
}