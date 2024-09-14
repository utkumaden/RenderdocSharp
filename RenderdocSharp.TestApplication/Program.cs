// This is modified from NogginBops' ImGUI controller repository.
// https://github.com/NogginBops/ImGui.NET_OpenTK_Sample

using System;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using glfwWindow = OpenTK.Windowing.GraphicsLibraryFramework.Window;

namespace RenderdocSharp.TestApplication
{
    public class Window : GameWindow
    {
        ImGuiController _controller = null!;

        public Window() : base(GameWindowSettings.Default, new NativeWindowSettings(){ Size = new Vector2i(1600, 900), APIVersion = new Version(3, 3) })
        { }

        protected override void OnLoad()
        {
            base.OnLoad();

            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);
            if (Renderdoc.IsAvailable)
            {
                Title += " (Renderdoc Available)";
                
                nint hdevice = 0, hwindow = 0;

                unsafe
                {
                    glfwWindow* wnd = WindowPtr;
                    
                    if (OperatingSystem.IsWindows())
                    {
                        hwindow = GLFW.GetWin32Window(wnd);
                        hdevice = GLFW.GetWGLContext(wnd);
                    }
                    else if (OperatingSystem.IsLinux())
                    {
                        hwindow = (nint)GLFW.GetGLXWindow(wnd);
                        hdevice = (nint)GLFW.GetGLXContext(wnd);
                    }
                    else if (OperatingSystem.IsMacOS())
                    {
                        hwindow = GLFW.GetCocoaWindow(wnd);
                        hdevice = GLFW.GetNSGLContext(wnd);
                    }
                    else
                    {
                        throw new Exception("Unsupported Platform");
                    }
                }

                Renderdoc.SetActiveWindow(hdevice, hwindow);
                // Renderdoc.StartFrameCapture(0, 0);
            }

            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
        }
        
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            // Update the opengl viewport
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

            // Tell ImGui of the new size
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _controller.Update(this, (float)e.Time);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            ImGui.Begin("Renderdoc Options", ImGuiWindowFlags.None);

            if (Renderdoc.IsAvailable)
            {
                PaintUI();
            }
            else
            {
                ImGui.Text("Renderdoc API not available.");
            }
                
            ImGui.End();

            _controller.Render();

            ImGuiController.CheckGLError("End of frame");

            SwapBuffers();
        }

        private void PaintUI()
        {
            ImGui.Text($"Renderdoc API Version {Renderdoc.Version!.ToString(3)}");

            PaintStatus();
            PaintOverlayBits();

            if (ImGui.BeginTabBar("Tabs"))
            {
                PaintCaptureOptions();
                PaintCaptures();

                ImGui.EndTabBar();
            }
        }

        private void PaintCaptures()
        {
            if (!ImGui.BeginTabItem("Captures"))
                return;

            ImGui.Text($"{Renderdoc.NumCaptures} Capture(s)");

            if (ImGui.Button("Launch Replay UI"))
            {
                Renderdoc.LaunchReplayUI(true, null);
            }
            ImGui.SameLine();
            if (ImGui.Button("Show Replay UI"))
            {
                Renderdoc.ShowReplayUI();
            }

            if (ImGui.BeginTable("Capture List", 3))
            {
                ImGui.TableSetupColumn("Index");
                ImGui.TableSetupColumn("File");
                ImGui.TableSetupColumn("Time");
                ImGui.TableHeadersRow();

                for (int i = 0; i < Renderdoc.NumCaptures; i++)
                {
                    Capture capture = Renderdoc.GetCapture(i)!.Value;

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(capture.Index.ToString());
                    ImGui.TableNextColumn();
                    ImGui.Text(capture.FileName);
                    ImGui.TableNextColumn();
                    ImGui.Text(capture.Timestamp.ToString());
                }

                ImGui.EndTable();
            }

            ImGui.EndTabItem();
        }

        private void PaintStatus()
        {
            bool x;

            if (ImGui.TreeNode("Status"))
            {
                x = Renderdoc.IsTargetControlConnected;
                ImGui.Checkbox("Target Control Connected", ref x);
                
                x = Renderdoc.IsFrameCapturing;
                ImGui.Checkbox("Capturing", ref x);

                ImGui.TreePop();
            }

        }

        private void PaintOverlayBits()
        {
            if (!ImGui.TreeNode("Overlay Options"))
                return;

            int bits = (int)Renderdoc.OverlayBits;
            
            bool update = false;

            update |= ImGui.CheckboxFlags("Enabled", ref bits, (int)OverlayBits.Enabled);
            update |= ImGui.CheckboxFlags("Frame Rate", ref bits, (int)OverlayBits.FrameRate);
            update |= ImGui.CheckboxFlags("Frame Number", ref bits, (int)OverlayBits.FrameNumber);
            update |= ImGui.CheckboxFlags("Capture List", ref bits, (int)OverlayBits.CaptureList);

            if (update)
                Renderdoc.OverlayBits = (OverlayBits)bits;

            ImGui.TreePop();
        }

        private int captures = 1;
        private void PaintCaptureOptions()
        {
            if (!ImGui.BeginTabItem("Capture Options"))
                return;
            
            PaintBoolCaptureOption(CaptureOption.AllowVsync);
            PaintBoolCaptureOption(CaptureOption.AllowFullscreen);
            PaintBoolCaptureOption(CaptureOption.ApiValidation);
            PaintBoolCaptureOption(CaptureOption.CaptureCallstacks);
            PaintBoolCaptureOption(CaptureOption.CaptureCallstacksOnlyDraws);
            PaintBoolCaptureOption(CaptureOption.VerifyBufferAccess);
            PaintBoolCaptureOption(CaptureOption.HookIntoChildren);
            PaintBoolCaptureOption(CaptureOption.RefAllSources);
            PaintBoolCaptureOption(CaptureOption.SaveAllInitials);
            PaintBoolCaptureOption(CaptureOption.CaptureAllCmdLists);
            PaintBoolCaptureOption(CaptureOption.DebugOutputMute);
            PaintBoolCaptureOption(CaptureOption.AllowUnsupportedVendorExtensions);

            {
                int delay = Renderdoc.GetCaptureOptionU32(CaptureOption.DelayForDebugger);
                if (ImGui.InputInt("Debugger Delay", ref delay))
                {
                    Renderdoc.SetCaptureOption(CaptureOption.DelayForDebugger, delay);
                }
            }

            {
                int memory = Renderdoc.GetCaptureOptionU32(CaptureOption.SoftMemoryLimit);

                if (ImGui.InputInt("Soft Memory Limit", ref memory))
                {
                    Renderdoc.SetCaptureOption(CaptureOption.SoftMemoryLimit, memory);
                }
            }

            {
                string str = Renderdoc.CaptureFilePathTemplate;
                if (ImGui.InputText("Capture File Path Template", ref str, ~0u))
                {
                    Renderdoc.CaptureFilePathTemplate = str;
                }
            }

            ImGui.BeginDisabled(Renderdoc.Version < new Version(1, 6));
            if (ImGui.InputInt("Capture Count", ref captures, 1, 10, ImGuiInputTextFlags.CharsDecimal))
            {
                captures = Math.Max(1, captures);
            }

            if (ImGui.Button("Capture Multiple"))
            {
                Renderdoc.TriggerMultiFrameCapture(captures);
            }
            ImGui.EndDisabled();

            ImGui.SameLine();
            if (ImGui.Button("Capture Single"))
            {
                Renderdoc.TriggerCapture();
            }

            ImGui.SameLine();
            if (Renderdoc.IsFrameCapturing)
            {
                if (ImGui.Button("Stop Capture##begin_capture"))
                {
                    Renderdoc.StartFrameCapture(0, 0);
                }
            }
            else 
            {
                if (ImGui.Button("Begin Capture##begin_capture"))
                {
                    Renderdoc.EndFrameCapture(0, 0);
                }
            }

            ImGui.SameLine();
            ImGui.BeginDisabled(Renderdoc.IsFrameCapturing && Renderdoc.Version < new Version(1, 3));
            if (ImGui.Button("Discard Capture"))
            {
                Renderdoc.DiscardFrameCapture(0, 0);
            }
            ImGui.EndDisabled();

            ImGui.EndTabItem();

            void PaintBoolCaptureOption(CaptureOption option)
            {
                bool value = Renderdoc.GetCaptureOptionU32(option) != 0;

                if (ImGui.Checkbox(option.ToString(), ref value))
                    Renderdoc.SetCaptureOption(option, value ? 1 : 0);
            }
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            
            
            _controller.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            
            _controller.MouseScroll(e.Offset);
        }

        public static void Main(string[] args)
        {
            new Window().Run();
        }
    }
}