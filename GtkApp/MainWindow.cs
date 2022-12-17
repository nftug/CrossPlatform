using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using SharedLib.Services;

namespace GtkApp;

public class MainWindow : Window
{
    [UI] private readonly Label _label1 = null!;
    [UI] private readonly Button _buttonHourUp = null!;
    [UI] private readonly Button _buttonHourDown = null!;
    [UI] private readonly Button _buttonMinUp = null!;
    [UI] private readonly Button _buttonMinDown = null!;
    [UI] private readonly Button _buttonSecUp = null!;
    [UI] private readonly Button _buttonSecDown = null!;
    [UI] private readonly Button _buttonToggleTimer = null!;
    [UI] private readonly Button _buttonResetTimer = null!;

    private readonly IKitchenTimerService _kitchenTimer = null!;

    public MainWindow(IKitchenTimerService kitchenTimer)
        : this(new Builder($"{nameof(MainWindow)}.glade"))
    {
        _kitchenTimer = kitchenTimer;
        _kitchenTimer.SecondsChanged += OnChangeSeconds;
        _kitchenTimer.TimerEnded += OnEndedTimer;

        DeleteEvent += OnDeleteWindow;
        _buttonToggleTimer.ButtonReleaseEvent += (_, _) => _kitchenTimer.ToggleTimer();
        _buttonResetTimer.ButtonReleaseEvent += (_, _) => _kitchenTimer.Status = TimerStatus.Stopped;
        _buttonHourUp.ButtonReleaseEvent += (_, _) => _kitchenTimer.Seconds += 3600;
        _buttonHourDown.ButtonReleaseEvent += (_, _) => _kitchenTimer.Seconds -= 3600;
        _buttonMinUp.ButtonReleaseEvent += (_, _) => _kitchenTimer.Seconds += 60;
        _buttonMinDown.ButtonReleaseEvent += (_, _) => _kitchenTimer.Seconds -= 60;
        _buttonSecUp.ButtonReleaseEvent += (_, _) => _kitchenTimer.Seconds++;
        _buttonSecDown.ButtonReleaseEvent += (_, _) => _kitchenTimer.Seconds--;

        _kitchenTimer.Seconds = 0;
    }

    private MainWindow(Builder builder) : base(builder.GetRawOwnedObject(nameof(MainWindow)))
    {
        builder.Autoconnect(this);
    }

    private void OnDeleteWindow(object sender, DeleteEventArgs a)
    {
        if (_kitchenTimer.Status != TimerStatus.Stopped)
        {
            var previousStatus = _kitchenTimer.Status;
            _kitchenTimer.Status = TimerStatus.Paused;

            var md = new MessageDialog(this,
                DialogFlags.DestroyWithParent, MessageType.Question,
                ButtonsType.YesNo, "タイマーが動作中です。\n終了しますか？"
            );
            var result = (ResponseType)md.Run();
            md.Destroy();

            if (result == ResponseType.No)
            {
                _kitchenTimer.Status = previousStatus;
                a.RetVal = true;
                return;
            }
        }

        _kitchenTimer.Status = TimerStatus.Stopped;
        Application.Quit();
    }

    private void OnChangeSeconds(object? sender, SecondsChangeEventArgs e)
    {
        Application.Invoke((_, _) =>
        {
            _label1.Text = TimeSpan.FromSeconds(e.Seconds).ToString(@"hh\:mm\:ss");

            Title = !e.IsStopped
                ? $"残り時間　{_label1.Text}{(e.IsPaused ? " (一時停止中)" : null)}"
                : "タイマー";

            _buttonToggleTimer.Label =
                e.IsActivated ? "Pause"
                : e.IsPaused ? "Restart"
                : "Start";

            _buttonToggleTimer.Sensitive = e.Seconds > 0;
            _buttonResetTimer.Sensitive = e.Seconds > 0 && !e.IsActivated;
            _buttonSecUp.Sensitive = !e.IsActivated;
            _buttonSecDown.Sensitive = e.Seconds > 0 && !e.IsActivated;
            _buttonMinUp.Sensitive = !e.IsActivated;
            _buttonMinDown.Sensitive = e.Seconds > 0 && !e.IsActivated;
            _buttonHourUp.Sensitive = !e.IsActivated;
            _buttonHourDown.Sensitive = e.Seconds > 0 && !e.IsActivated;
        });
    }

    private void OnEndedTimer(object? sender, EventArgs e)
    {
        Application.Invoke((_, _) =>
        {
            var dialog = new MessageDialog(this,
                DialogFlags.DestroyWithParent, MessageType.Info,
                ButtonsType.Ok, "時間です"
            );
            dialog.Run();
            dialog.Destroy();
        });
    }
}
