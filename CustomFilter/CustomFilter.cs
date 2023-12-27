using System.Numerics;
using AngouriMath;
using AngouriMath.Core;
using AngouriMath.Extensions;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace CustomFilter;

[PluginName("Custom Filter")]
public class CustomFilter : IPositionedPipelineElement<IDeviceReport>
{
    private const string TOOLTIP =  "x = The X coordinate\n" +
                                    "y = The Y coordinate\n" +
                                    "p = The pressure\n" +
                                    "tx = The tilt X component\n" +
                                    "ty = The tilt Y component\n" +
                                    "d = The hover distance\n" +
                                    "lx = The last X coordinate\n" +
                                    "ly = The last Y coordinate\n" +
                                    "lp = The last pressure\n" +
                                    "ltx = The last tilt X component\n" +
                                    "lty = The last tilt Y component\n" +
                                    "ld = The last hover distance\n" +
                                    "mx = Max X coordinate\n" +
                                    "my = Max Y coordinate\n" + 
                                    "mp = Max pressure\n" +
                                    "cx = Last computed X coordinate\n" +
                                    "cy = Last computed Y coordinate\n" +
                                    "cp = Last computed pressure\n";

    private static readonly string[] variables = { "x", "y", "p", "tx", "ty", "d", "lx", "ly", "lp", "ltx", "lty", "ld", "mx", "my", "mp", "cx", "cy", "cp" };

    public FastExpression CalcX = ((Entity)"x").Compile(variables);
    public FastExpression CalcY = ((Entity)"y").Compile(variables);
    public FastExpression CalcP = ((Entity)"p").Compile(variables);
    public FastExpression CalcTX = ((Entity)"tx").Compile(variables);
    public FastExpression CalcTY = ((Entity)"ty").Compile(variables);

    public Vector2 LastPosition = Vector2.Zero;
    public uint LastPressure = 0;
    public Vector2 LastTilt = Vector2.Zero;
    public uint LastHoverDistance = 0;
    public Vector2 LastComputedPosition = Vector2.Zero;
    public uint LastComputedPressure = 0;

    /// <summary>
    /// Recompiles all polynomials to a function.
    /// </summary>
    [OnDependencyLoad]
    public void Recompile()
    {
        Entity xExpr = XFunc;
        Entity yExpr = YFunc;
        Entity pExpr = PFunc;
        Entity txExpr = TXFunc;
        Entity tyExpr = TYFunc;
        try
        {
            CalcX = xExpr.Compile(variables);
        }
        catch (Exception ex)
        {
            CalcX = ((Entity)"x").Compile(variables);
            Log.Exception(ex);
            Log.WriteNotify("Custom Filter", "Error while compiling X polynomial! Resetting...", LogLevel.Error);
        }

        try
        {
            CalcY = yExpr.Compile(variables);
        }
        catch (Exception ex)
        {
            CalcY = ((Entity)"y").Compile(variables);
            Log.Exception(ex);
            Log.WriteNotify("Custom Filter", "Error while compiling Y polynomial! Resetting...", LogLevel.Error);
        }

        try
        {
            CalcP = pExpr.Compile(variables);
        }
        catch (Exception ex)
        {
            CalcP = ((Entity)"p").Compile(variables);
            Log.Exception(ex);
            Log.WriteNotify("Custom Filter", "Error while compiling P polynomial! Resetting...", LogLevel.Error);
        }

        try
        {
            CalcTX = txExpr.Compile(variables);
        }
        catch (Exception ex)
        {
            CalcTX = ((Entity)"tx").Compile(variables);
            Log.Exception(ex);
            Log.WriteNotify("Custom Filter", "Error while compiling TX polynomial! Resetting...", LogLevel.Error);
        }

        try
        {
            CalcTY = tyExpr.Compile(variables);
        }
        catch (Exception ex)
        {
            CalcTY = ((Entity)"ty").Compile(variables);
            Log.Exception(ex);
            Log.WriteNotify("Custom Filter", "Error while compiling TY polynomial! Resetting...", LogLevel.Error);
        }

        Log.Debug("Custom Filter", "Recompiled all functions");
    }

    public void Consume(IDeviceReport value)
    {
        var digitizer = TabletReference.Properties.Specifications.Digitizer;
        var pen = TabletReference.Properties.Specifications.Pen;

        Vector2 position = Vector2.Zero;
        uint pressure = 0;
        Vector2 tilt = Vector2.Zero;
        uint hoverDistance = 0;

        if (value is ITiltReport setTiltReport)
        {
            tilt = setTiltReport.Tilt;
        }

        if (value is IProximityReport setProximityReport)
        {
            hoverDistance = setProximityReport.HoverDistance;
        }

        if (value is ITabletReport tabletReport)
        {
            LastPosition = tabletReport.Position;
            LastPressure = tabletReport.Pressure;

            position = 

            tabletReport.Position = new Vector2(
                (float)CalcX.Call(tabletReport.Position.X, tabletReport.Position.Y, tabletReport.Pressure, tilt.X, tilt.Y, hoverDistance, LastPosition.X, LastPosition.Y, LastPressure, LastTilt.X, LastTilt.Y, LastHoverDistance, digitizer.MaxX, digitizer.MaxY, pen.MaxPressure, LastComputedPosition.X, LastComputedPosition.Y, LastComputedPressure).Real,
                (float)CalcY.Call(tabletReport.Position.X, tabletReport.Position.Y, tabletReport.Pressure, tilt.X, tilt.Y, hoverDistance, LastPosition.X, LastPosition.Y, LastPressure, LastTilt.X, LastTilt.Y, LastHoverDistance, digitizer.MaxX, digitizer.MaxY, pen.MaxPressure, LastComputedPosition.X, LastComputedPosition.Y, LastComputedPressure).Real
            );
            tabletReport.Pressure = (uint)CalcP.Call(tabletReport.Position.X, tabletReport.Position.Y, tabletReport.Pressure, tilt.X, tilt.Y, hoverDistance, LastPosition.X, LastPosition.Y, LastPressure, LastTilt.X, LastTilt.Y, LastHoverDistance, digitizer.MaxX, digitizer.MaxY, pen.MaxPressure, LastComputedPosition.X, LastComputedPosition.Y, LastComputedPressure).Real;

            LastComputedPosition = tabletReport.Position;
            LastComputedPressure = tabletReport.Pressure;

            value = tabletReport;
        }

        if (value is ITiltReport tiltReport)
        {
            LastTilt = tiltReport.Tilt;

            tiltReport.Tilt = new Vector2(
                (float)CalcTX.Call(position.X, position.Y, pressure, tilt.X, tilt.Y, hoverDistance, LastPosition.X, LastPosition.Y, LastPressure, LastTilt.X, LastTilt.Y, LastHoverDistance, digitizer.MaxX, digitizer.MaxY, pen.MaxPressure, LastComputedPosition.X, LastComputedPosition.Y, LastComputedPressure).Real,
                (float)CalcTY.Call(position.X, position.Y, pressure, tilt.X, tilt.Y, hoverDistance, LastPosition.X, LastPosition.Y, LastPressure, LastTilt.X, LastTilt.Y, LastHoverDistance, digitizer.MaxX, digitizer.MaxY, pen.MaxPressure, LastComputedPosition.X, LastComputedPosition.Y, LastComputedPressure).Real
            );

            value = tiltReport;
        }

        if (value is IProximityReport proximityReport)
        {
            LastHoverDistance = proximityReport.HoverDistance;
        }

        Emit?.Invoke(value);
    }

    public event Action<IDeviceReport> Emit;
    public PipelinePosition Position => PipelinePosition.PreTransform;

    [Property("X coordinate polynomial"), DefaultPropertyValue("x"), ToolTip(
         "A polynomial that calculates the X coordinate\n" + TOOLTIP)]
    public string XFunc { get; set; }

    [Property("Y coordinate polynomial"), DefaultPropertyValue("y"), ToolTip(
         "A polynomial that calculates the Y coordinate\n" + TOOLTIP)]
    public string YFunc { get; set; }

    [Property("Pressure polynomial"), DefaultPropertyValue("p"), ToolTip(
         "A polynomial that calculates the pressure\n" + TOOLTIP)]
    public string PFunc { get; set; }

    [Property("X tilt polynomial"), DefaultPropertyValue("tx"), ToolTip(
         "A polynomial that calculates the X tilt\n" + TOOLTIP)]
    public string TXFunc { get; set; }

    [Property("Y tilt polynomial"), DefaultPropertyValue("ty"), ToolTip(
         "A polynomial that calculates the Y tilt\n" + TOOLTIP)]
    public string TYFunc { get; set; }

    [TabletReference]
    public TabletReference TabletReference { get; set; }
}