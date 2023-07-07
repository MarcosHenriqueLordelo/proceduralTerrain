using System.Collections;
using System.Threading;

public class WaitForBackgroundThread : IEnumerator {
    private Thread thread;

    public WaitForBackgroundThread(Thread thread) {
        this.thread = thread;
    }

    public object Current {
        get { return null; }
    }

    public bool MoveNext() {
        return thread.IsAlive;
    }

    public void Reset() {
    }
}
