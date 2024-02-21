using System.IO.Enumeration;
using System.Numerics;
using ClickableTransparentOverlay;
using ImGuiNET;
using ModernImgui.Icons;

namespace ModernImgui.Rendering;

public class Renderer : Overlay
{
    private readonly string[] _comboItems = { "Item 1", "Item 2", "Item 3", "Item 4" };
    private readonly string[] _headerItems = { "Home", "Aim", "Esp", "Misc" };
    private bool _checkBox, _checkBox2;
    private int _comboBox, _sliderScalar;

    private int _selectedHeader;

    protected override unsafe Task PostInitialized()
    {
        ReplaceFont(config =>
        {
            var io = ImGui.GetIO();
            
            if (File.Exists("Fonts\\lucide.ttf") && File.Exists("Fonts\\Inter-SemiBold.ttf"))
            {
                Console.WriteLine("Font exists");
                io.Fonts.AddFontFromFileTTF(
                    "Fonts\\Inter-SemiBold.ttf", 20, config,
                    io.Fonts.GetGlyphRangesDefault());

                ushort[] glyphRanges = [Lucide.IconMin, Lucide.IconMax16, 0];

                config->MergeMode = 1;
                config->OversampleH = 1;
                config->OversampleV = 1;
                config->PixelSnapH = 1;
                fixed (ushort* p = &glyphRanges[0])
                {
                    io.Fonts.AddFontFromFileTTF(
                        "Fonts\\lucide.ttf", 20, config, new IntPtr(p));
                }
            }
        });
        Style();
        return base.PostInitialized();
    }
    private void VerticalCenteredIconWithText(string icon, string text,float iconScale = 1.1f)
    {
        var iconSizeHeight = 18*iconScale;
        var textSize = ImGui.CalcTextSize(text).Y;
        ImGui.SetWindowFontScale(iconScale);
        ImGui.Text(icon);
        ImGui.SetWindowFontScale(1f);
        ImGui.SameLine();
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + (iconSizeHeight - textSize) / 2);
        ImGui.Text(text);
    }
   

    protected override void Render()
    {
        var dark = new Vector4(0.1725490242242813f, 0.1921568661928177f, 0.2352941185235977f, 1.0f);
        var light = new Vector4(0.12f, 0.14f, 0.15f, 1.0f);
        ImGui.SetNextWindowSize(new Vector2(500, 370));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 9);
        ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(0.5f, 0.5f));
        ImGui.PushStyleColor(ImGuiCol.WindowBg, light);
        ImGui.Begin("catrine", ImGuiWindowFlags.NoDecoration);
        {
            var drawList = ImGui.GetWindowDrawList();
            var windowPos = ImGui.GetWindowPos();
            var windowSize = ImGui.GetWindowSize();
            var headerHeight = 50;
            drawList.AddRectFilled(windowPos, new Vector2(windowPos.X + windowSize.X, windowPos.Y + headerHeight),
                ImGui.GetColorU32(dark), 9.0f, ImDrawFlags.RoundCornersTop);
            var circleCenter = new Vector2(windowPos.X + 25, windowPos.Y + 25);
            drawList.AddCircleFilled(circleCenter, 10.5f,
                ImGui.GetColorU32(new Vector4(0.3294117748737335f, 0.6666666865348816f, 0.8588235378265381f, 1.0f)));
            ImGui.SetCursorPos(new Vector2(45, 0));
            ImGui.BeginChild("header", new Vector2(-1, headerHeight));
            {
                var windowHeight = ImGui.GetWindowHeight();
                var height = ImGui.GetTextLineHeight();
                ImGui.SetCursorPosY((windowHeight - height) / 2.0f);
                ImGui.Text("VYNXC");
                ImGui.SameLine(0, 30);
                var textColor = ImGui.GetColorU32(ImGuiCol.Text);
                var textColorVec = ImGui.ColorConvertU32ToFloat4(textColor);
                textColorVec.W /= 2;

                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 0, 0, 0));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0, 0, 0, 0));
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
                for (var i = 0; i < _headerItems.Length; i++)
                {
                    var headerString = _headerItems[i];
                    ImGui.SameLine(0, 15);

                    if (i == _selectedHeader)
                    {
                        SetCursorCenterY();
                        ImGui.Button(headerString);
                    }
                    else
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, textColorVec);
                        {
                            SetCursorCenterY();

                            if (ImGui.Button(headerString)) _selectedHeader = i;
                        }

                        ImGui.PopStyleColor();
                    }
                }

                ImGui.PopStyleColor(3);
                ImGui.PopStyleVar();
            }
            ImGui.End();
            ImGui.SetCursorPosY(headerHeight + 10);
            ImGui.BeginChild("content", new Vector2(-1, -1));
            {
                switch (_selectedHeader)
                {
                    case 0:
                        RenderTabOne();
                        break;
                    case 1:
                        RenderTabTwo();
                        break;
                    case 2:
                        RenderTabThree();
                        break;
                    case 3:
                        RenderTabFour();
                        break;
                    default:
                        ImGui.Text("No content");
                        break;
                }
            }
        }
        ImGui.End();

        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor();
    }

    private void RenderTabOne()
    {
        Text("Tab header 1", 0.6f);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(.2f, 0.2f, 0.2f, 1));
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered,
            new Vector4(0.3294117748737335f, 0.6666666865348816f, 0.8588235378265381f, .4f));
        ImGui.Checkbox("Checkbox", ref _checkBox);
        ImGui.Checkbox("Checkbox 2", ref _checkBox2);
        ImGui.PopStyleColor(2);
        ImGui.Text(" Combo box");
        ImGui.PushItemWidth(150);
        ImGui.PushStyleColor(ImGuiCol.PopupBg,
            new Vector4(0.1725490242242813f, 0.1921568661928177f, 0.2352941185235977f, 1.0f));
        ImGui.Combo("##w", ref _comboBox, _comboItems, _comboItems.Length, 4);
        ImGui.PopStyleColor();
        var sizeOne = ImGui.CalcTextSize(" Slider").X;
        var sizeTwo = ImGui.CalcTextSize($"{_sliderScalar}%%").X;
        ImGui.Text(" Slider");
        ImGui.SameLine();
        ImGui.Dummy(new Vector2(150 - (sizeOne + sizeTwo), 1));
        ImGui.SameLine();
        ImGui.Text($"{_sliderScalar}%%");
        ImGui.SliderInt("##slider", ref _sliderScalar, 0, 100, "");
        ImGui.PopItemWidth();
    }

    private void RenderTabTwo()
    {
        Text("Tab header 2", 0.6f);
        VerticalCenteredIconWithText(Lucide.Home, "Home");
        VerticalCenteredIconWithText(Lucide.User, "User");
        VerticalCenteredIconWithText(Lucide.Settings, "Settings");
        VerticalCenteredIconWithText(Lucide.LogOut, "Log out");
    }

    private void RenderTabThree()
    {
    }

    private void RenderTabFour()
    {
    }

    private void SetCursorCenterY()
    {
        var windowHeight = ImGui.GetWindowHeight();
        var height = ImGui.GetTextLineHeight();
        ImGui.SetCursorPosY((windowHeight - height) / 2.0f);
    }

    private void Text(string text, float opacity = 1)
    {
        var textColor = ImGui.GetColorU32(ImGuiCol.Text);
        var textColorVec = ImGui.ColorConvertU32ToFloat4(textColor);
        textColorVec.W = opacity;
        ImGui.PushStyleColor(ImGuiCol.Text, textColorVec);
        {
            ImGui.Text(text);
        }
        ImGui.PopStyleColor();
    }

    private void Style()
    {
        var style = ImGui.GetStyle();

        style.Alpha = 1.0f;
        style.DisabledAlpha = 0.6000000238418579f;
        style.WindowPadding = new Vector2(8.0f, 8.0f);
        style.WindowRounding = 7.0f;
        style.WindowBorderSize = 1.0f;
        style.WindowMinSize = new Vector2(32.0f, 32.0f);
        style.WindowTitleAlign = new Vector2(0.0f, 0.5f);
        style.WindowMenuButtonPosition = ImGuiDir.Left;
        style.ChildRounding = 4.0f;
        style.ChildBorderSize = 1.0f;
        style.PopupRounding = 4.0f;
        style.PopupBorderSize = 1.0f;
        style.FramePadding = new Vector2(5.0f, 2.0f);
        style.FrameRounding = 3.0f;
        style.FrameBorderSize = 1.0f;
        style.ItemSpacing = new Vector2(6.0f, 6.0f);
        style.ItemInnerSpacing = new Vector2(6.0f, 6.0f);
        style.CellPadding = new Vector2(6.0f, 6.0f);
        style.IndentSpacing = 25.0f;
        style.ColumnsMinSpacing = 6.0f;
        style.ScrollbarSize = 15.0f;
        style.ScrollbarRounding = 9.0f;
        style.GrabMinSize = 10.0f;
        style.GrabRounding = 3.0f;
        style.TabRounding = 4.0f;
        style.TabBorderSize = 1.0f;
        style.TabMinWidthForCloseButton = 0.0f;
        style.ColorButtonPosition = ImGuiDir.Right;
        style.ButtonTextAlign = new Vector2(0.5f, 0.5f);
        style.SelectableTextAlign = new Vector2(0.0f, 0.0f);

        style.Colors[(int)ImGuiCol.Text] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        style.Colors[(int)ImGuiCol.TextDisabled] =
            new Vector4(0.4980392158031464f, 0.4980392158031464f, 0.4980392158031464f, 1.0f);
        style.Colors[(int)ImGuiCol.WindowBg] =
            new Vector4(0.09803921729326248f, 0.09803921729326248f, 0.09803921729326248f, 1.0f);
        style.Colors[(int)ImGuiCol.ChildBg] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(0.1882352977991104f, 0.1882352977991104f, 0.1882352977991104f,
            0.9200000166893005f);
        style.Colors[(int)ImGuiCol.Border] = new Vector4(0.1882352977991104f, 0.1882352977991104f, 0.1882352977991104f,
            0.2899999916553497f);
        style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.0f, 0.0f, 0.0f, 0.239999994635582f);
        style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(0.0470588244497776f, 0.0470588244497776f, 0.0470588244497776f,
            0.5400000214576721f);
        style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.1882352977991104f, 0.1882352977991104f,
            0.1882352977991104f, 0.5400000214576721f);
        style.Colors[(int)ImGuiCol.FrameBgActive] =
            new Vector4(0.2000000029802322f, 0.2196078449487686f, 0.2274509817361832f, 1.0f);
        style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        style.Colors[(int)ImGuiCol.TitleBgActive] =
            new Vector4(0.05882352963089943f, 0.05882352963089943f, 0.05882352963089943f, 1.0f);
        style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        style.Colors[(int)ImGuiCol.MenuBarBg] =
            new Vector4(0.1372549086809158f, 0.1372549086809158f, 0.1372549086809158f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.0470588244497776f, 0.0470588244497776f,
            0.0470588244497776f, 0.5400000214576721f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.3372549116611481f, 0.3372549116611481f,
            0.3372549116611481f, 0.5400000214576721f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.4000000059604645f, 0.4000000059604645f,
            0.4000000059604645f, 0.5400000214576721f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.5568627715110779f, 0.5568627715110779f,
            0.5568627715110779f, 0.5400000214576721f);
        style.Colors[(int)ImGuiCol.CheckMark] =
            new Vector4(0.3294117748737335f, 0.6666666865348816f, 0.8588235378265381f, 1.0f);
        style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.3372549116611481f, 0.3372549116611481f,
            0.3372549116611481f, 0.5400000214576721f);
        style.Colors[(int)ImGuiCol.SliderGrabActive] =
            new Vector4(0.3294117748737335f, 0.6666666865348816f, 0.8588235378265381f, 1.0f);
        style.Colors[(int)ImGuiCol.Button] = new Vector4(0.0470588244497776f, 0.0470588244497776f, 0.0470588244497776f,
            0.5400000214576721f);
        style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.1882352977991104f, 0.1882352977991104f,
            0.1882352977991104f, 0.5400000214576721f);
        style.Colors[(int)ImGuiCol.ButtonActive] =
            new Vector4(0.2000000029802322f, 0.2196078449487686f, 0.2274509817361832f, 1.0f);
        style.Colors[(int)ImGuiCol.Header] = new Vector4(0.0f, 0.0f, 0.0f, 0.5199999809265137f);
        style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.0f, 0.0f, 0.0f, 0.3600000143051147f);
        style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.2000000029802322f, 0.2196078449487686f,
            0.2274509817361832f, 0.3300000131130219f);
        style.Colors[(int)ImGuiCol.Separator] = new Vector4(0.2784313857555389f, 0.2784313857555389f,
            0.2784313857555389f, 0.2899999916553497f);
        style.Colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.4392156898975372f, 0.4392156898975372f,
            0.4392156898975372f, 0.2899999916553497f);
        style.Colors[(int)ImGuiCol.SeparatorActive] =
            new Vector4(0.4000000059604645f, 0.4392156898975372f, 0.4666666686534882f, 1.0f);
        style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.2784313857555389f, 0.2784313857555389f,
            0.2784313857555389f, 0.2899999916553497f);
        style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.4392156898975372f, 0.4392156898975372f,
            0.4392156898975372f, 0.2899999916553497f);
        style.Colors[(int)ImGuiCol.ResizeGripActive] =
            new Vector4(0.4000000059604645f, 0.4392156898975372f, 0.4666666686534882f, 1.0f);
        style.Colors[(int)ImGuiCol.Tab] = new Vector4(0.0f, 0.0f, 0.0f, 0.5199999809265137f);
        style.Colors[(int)ImGuiCol.TabHovered] =
            new Vector4(0.1372549086809158f, 0.1372549086809158f, 0.1372549086809158f, 1.0f);
        style.Colors[(int)ImGuiCol.TabActive] = new Vector4(0.2000000029802322f, 0.2000000029802322f,
            0.2000000029802322f, 0.3600000143051147f);
        style.Colors[(int)ImGuiCol.TabUnfocused] = new Vector4(0.0f, 0.0f, 0.0f, 0.5199999809265137f);
        style.Colors[(int)ImGuiCol.TabUnfocusedActive] =
            new Vector4(0.1372549086809158f, 0.1372549086809158f, 0.1372549086809158f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        style.Colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.0f, 0.0f, 0.0f, 0.5199999809265137f);
        style.Colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.0f, 0.0f, 0.0f, 0.5199999809265137f);
        style.Colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.2784313857555389f, 0.2784313857555389f,
            0.2784313857555389f, 0.2899999916553497f);
        style.Colors[(int)ImGuiCol.TableRowBg] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        style.Colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(1.0f, 1.0f, 1.0f, 0.05999999865889549f);
        style.Colors[(int)ImGuiCol.TextSelectedBg] =
            new Vector4(0.2000000029802322f, 0.2196078449487686f, 0.2274509817361832f, 1.0f);
        style.Colors[(int)ImGuiCol.DragDropTarget] =
            new Vector4(0.3294117748737335f, 0.6666666865348816f, 0.8588235378265381f, 1.0f);
        style.Colors[(int)ImGuiCol.NavHighlight] = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.0f, 0.0f, 0.0f, 0.699999988079071f);
        style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(1.0f, 0.0f, 0.0f, 0.2000000029802322f);
        style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(1.0f, 0.0f, 0.0f, 0.3499999940395355f);
    }
}