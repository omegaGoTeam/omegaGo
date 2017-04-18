using OmegaGo.Core.Game.Markup;

namespace OmegaGo.Core.Game.Tools
{
    public sealed class SimpleMarkupTool : IMarkupTool
    {
        public SimpleMarkupKind Markup { get; }

        public SimpleMarkupTool(SimpleMarkupKind markupKind)
        {
            Markup = markupKind;
        }

        public void Execute(IToolServices toolService)
        {
            Position position = toolService.PointerOverPosition;
            MarkupInfo markups = toolService.Node.Markups;

            markups.RemoveMarkupOnPosition(position);

            if (Markup == SimpleMarkupKind.Circle)
                markups.AddMarkup<Circle>(new Circle(position));
            if (Markup == SimpleMarkupKind.Cross)
                markups.AddMarkup<Cross>(new Cross(position));
            if (Markup == SimpleMarkupKind.Square)
                markups.AddMarkup<Square>(new Square(position));
            if (Markup == SimpleMarkupKind.Triangle)
                markups.AddMarkup<Triangle>(new Triangle(position));
        }

        public IMarkup GetShadowItem(IToolServices toolServices)
        {
            if (Markup == SimpleMarkupKind.Circle)
                return new Circle(toolServices.PointerOverPosition);
            if (Markup == SimpleMarkupKind.Cross)
                return new Cross(toolServices.PointerOverPosition);
            if (Markup == SimpleMarkupKind.Square)
                return new Square(toolServices.PointerOverPosition);
            if (Markup == SimpleMarkupKind.Triangle)
                return new Triangle(toolServices.PointerOverPosition);

            return null;
        }

    }
}
