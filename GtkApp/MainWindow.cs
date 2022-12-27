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
        _kitchenTimer.CombinedStatus.Subscribe(OnChangeTimerStatus);
        _kitchenTimer.TimerEnded += OnEndedTimer;

        DeleteEvent += OnDeleteWindow;
        _buttonToggleTimer.ButtonReleaseEvent += (_, _) => _kitchenTimer.ToggleTimer();
        _buttonResetTimer.ButtonReleaseEvent += (_, _) => _kitchenTimer.Status.Value = TimerStatus.Stopped;
        _buttonHourUp.ButtonReleaseEvent += (_, _) => _kitchenTimer.Second.Value += 3600;
        _buttonHourDown.ButtonReleaseEvent += (_, _) => _kitchenTimer.Second.Value -= 3600;
        _buttonMinUp.ButtonReleaseEvent += (_, _) => _kitchenTimer.Second.Value += 60;
        _buttonMinDown.ButtonReleaseEvent += (_, _) => _kitchenTimer.Second.Value -= 60;
        _buttonSecUp.ButtonReleaseEvent += (_, _) => _kitchenTimer.Second.Value++;
        _buttonSecDown.ButtonReleaseEvent += (_, _) => _kitchenTimer.Second.Value--;
    }

    private MainWindow(Builder builder) : base(builder.GetRawOwnedObject(nameof(MainWindow)))
    {
        builder.Autoconnect(this);
    }

    private void OnDeleteWindow(object sender, DeleteEventArgs a)
    {
        if (_kitchenTimer.Status.Value != TimerStatus.Stopped)
        {
            var previousStatus = _kitchenTimer.Status.Value;
            _kitchenTimer.Status.Value = TimerStatus.Paused;

            var md = new MessageDialog(this,
                DialogFlags.DestroyWithParent, MessageType.Question,
                ButtonsType.YesNo, "タイマーが動作中です。\n終了しますか？"
            );
            var result = (ResponseType)md.Run();
            md.Destroy();

            if (result == ResponseType.No)
            {
                _kitchenTimer.Status.Value = previousStatus;
                a.RetVal = true;
                return;
            }
        }

        Application.Quit();
    }

    private void OnChangeTimerStatus(TimerCombinedStatus? e)
    {
        if (e == null) return;

        Application.Invoke((_, _) =>
        {
            _label1.Text = TimeSpan.FromSeconds(e.Second).ToString(@"hh\:mm\:ss");

            Title = !e.IsStopped
                ? $"残り時間　{_label1.Text}{(e.IsPaused ? " (一時停止中)" : null)}"
                : "タイマー";

            _buttonToggleTimer.Label =
                e.IsActivated ? "Pause"
                : e.IsPaused ? "Restart"
                : "Start";

            _buttonToggleTimer.Sensitive = e.StartStopEnabled;
            _buttonResetTimer.Sensitive = e.ResetEnabled;
            _buttonSecUp.Sensitive = e.SecUpEnabled;
            _buttonSecDown.Sensitive = e.SecDownEnabled;
            _buttonMinUp.Sensitive = e.MinUpEnabled;
            _buttonMinDown.Sensitive = e.MinDownEnabled;
            _buttonHourUp.Sensitive = e.HourUpEnabled;
            _buttonHourDown.Sensitive = e.HourDownEnabled;
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
