using System.Drawing;

namespace SnapDesk.Interfaces;
public interface IScreenshotService
{
    void CaptureAndSave();
    void PrintSavedImageNames();
    void CaptureRegionAndSave(Rectangle region);

}
