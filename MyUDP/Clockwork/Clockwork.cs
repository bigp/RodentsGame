using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUDP.Clock {

    using ActionGear = Action<Gear>;

    public enum EGearTimeMode {
        DEFAULT,
        FRAME_BASED,
        TIME_BASED
    }

    //Master gear
    public class Clockwork : Gear {
        private Timer _internalTimer;
        private DateTime _lastDateTime;

        public Clockwork() : base() {
            _timeMode = EGearTimeMode.TIME_BASED;
            name = "*MASTER*";
        }

        public Clockwork StartAutoUpdate(int callsPerSecond=10) {
            StopAutoUpdate();

            _lastDateTime = DateTime.Now;
            int interval = (int) (1000 / callsPerSecond);
            _internalTimer = Utils.setTimeout(InternalUpdate, interval, true);

            return this;
        }

        public void StopAutoUpdate() {
            if (_internalTimer != null) {
                _internalTimer.Dispose();
            }
        }

        private void InternalUpdate(object state) {
            DateTime nowDatetime = DateTime.Now;
            TimeSpan diffTime = nowDatetime - _lastDateTime;
            float seconds = (float) diffTime.TotalSeconds;

            Log.traceClear();
            Log.trace(seconds);

            this.UpdateTime(seconds, 1);

            Log.BufferOutput();
            _lastDateTime = nowDatetime;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////

    //Gears can update by TIME or by TICKS (or multiple of TICKS even).
    public class Gear {
        private List<Gear> _gears;

        public string name;
        public bool isEnabled = true;
        public bool isIteratingCallbacks = true;

        private int _depth;
        internal EGearTimeMode _timeMode = EGearTimeMode.FRAME_BASED;

        internal float _timeScale = 1;
        public float timeScale {
            get { return _timeScale; }
            set { _timeScale = value; }
        }

        private float _timeCounter = 0;
        private float _timeCounterReset = 1;
        private int _frameCounter = 0;
        private int _frameCounterReset = 1;

        private int _preID = 0;
        private int _postID = 0;
        private List<ActionGear> _preCallbacks;
        private List<ActionGear> _postCallbacks;
        

        public Gear() {
            _gears = new List<Gear>();
            _preCallbacks = new List<ActionGear>();
            _postCallbacks = new List<ActionGear>();
        }

        public Gear SetParams(float counterReset = -1, bool isEnabled = true, EGearTimeMode timeMode = EGearTimeMode.DEFAULT) {
            if (timeMode != EGearTimeMode.DEFAULT) {
                this._timeMode = timeMode;
            }

            this.isEnabled = isEnabled;

            if (counterReset > 0f) {
                if (this._timeMode == EGearTimeMode.FRAME_BASED) {
                    this._frameCounterReset = (int)counterReset;
                    this._frameCounter = this._frameCounterReset;
                } else {
                    this._timeCounterReset = counterReset;
                    this._timeCounter = this._timeCounterReset;
                }
            }

            return this;
        }

        public Gear IsIteratingCallbacks(bool isIteratingCallbacks=true) {
            this.isIteratingCallbacks = isIteratingCallbacks;
            return this;
        }

        public Gear AddGear(string name) {
            Gear newGear = AddGear();
            newGear.name = name;
            return newGear;
        }

        public Gear AddGear(Gear newGear=null) {
            if(newGear==null) newGear = new Gear();
            _gears.Add(newGear);
            newGear._depth = this._depth+1;
            return newGear;
        }

        public void AddInterleaving(params ActionGear[] callbacks) {
            int len = callbacks.Length;
            var offset = 0;
            foreach (ActionGear cb in callbacks) {
                Gear childGear = AddGear("interleave " + offset).AddListener(cb);
                childGear._timeMode = EGearTimeMode.FRAME_BASED;
                childGear._frameCounterReset = len;
                childGear._frameCounter = offset;
                offset++;
            }
        }

        public Gear AddListeners(bool isPre, params ActionGear[] OnCompletes) {
            if(isPre) {
                _preCallbacks.AddRange(OnCompletes);
            } else {
                _postCallbacks.AddRange(OnCompletes);
            }
            
            return this;
        }

        public Gear AddListener(ActionGear OnComplete, bool isPre=true) {
            if (isPre) {
                _preCallbacks.Add(OnComplete);
            } else {
                _postCallbacks.Add(OnComplete);
            }
            return this;
        }

        public void ClearListeners() {
            _preCallbacks.Clear();
            _postCallbacks.Clear();
        }

        public Gear UpdateTime(float deltaTime, int frames) {
            if(_timeScale<=0 || (deltaTime<=0 && frames<=0)) return this;

            float adjustedDeltaTime = deltaTime * _timeScale;

            int iterations = 0;

            switch(_timeMode) {
                case EGearTimeMode.FRAME_BASED:
                    _frameCounter -= frames;
                    while (_frameCounter <= 0) {
                        _frameCounter += _frameCounterReset;
                        iterations++;
                    }
                    break;
                case EGearTimeMode.TIME_BASED:
                    _timeCounter -= adjustedDeltaTime < 0 ? 0 : adjustedDeltaTime;
                    while (_timeCounter <= 0) {
                        _timeCounter += _timeCounterReset;
                        iterations++;
                    }
                    break;
                default: Log.BufferAdd("Unknown time-mode for gear: " + this.name);
                    break;
            }

            for(var i=iterations; --i>=0;) Trigger(_preCallbacks, ref _preID);

            foreach (Gear gear in _gears) {
                gear.UpdateTime(adjustedDeltaTime, frames);
            }

            for (var j=iterations; --j>=0;) Trigger(_postCallbacks, ref _postID);

            return this;
        }

        private Gear Trigger(List<ActionGear> cbList, ref int counter) {
            if(cbList==null || cbList.Count==0) return this;

            if(isIteratingCallbacks) {
                cbList[counter](this);
                counter = (counter + 1) % cbList.Count;
            } else {
                foreach(ActionGear cb in cbList) {
                    cb(this);
                }
            }

            return this;
        }
    }
}
