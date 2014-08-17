﻿using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Forms;
using JuliusSweetland.ETTA.Extensions;
using JuliusSweetland.ETTA.Models;
using JuliusSweetland.ETTA.Properties;

namespace JuliusSweetland.ETTA.Observables.PointAndKeyValueSources
{
    public class MousePositionSource : IPointAndKeyValueSource
    {
        private readonly IObservable<Timestamped<PointAndKeyValue?>> sequence;

        public MousePositionSource(TimeSpan pointTtl)
        {
            sequence = Observable
                .Interval(Settings.Default.PointsMousePositionSampleInterval)
                .Select(_ => new Point(Cursor.Position.X, Cursor.Position.Y))
                .Timestamp()
                .PublishLivePointsOnly(pointTtl)
                .Select(tp => new Timestamped<PointAndKeyValue?>(tp.Value.ToPointAndKeyValue(PointToKeyValueMap), tp.Timestamp))
                .Replay(1) //Buffer one value for every subscriber so there is always a 'most recent' point available
                .RefCount();
        }
        
        public Dictionary<Rect, KeyValue> PointToKeyValueMap { private get; set; }

        public IObservable<Timestamped<PointAndKeyValue?>> Sequence { get { return sequence; } }
    }
}