﻿using System.ComponentModel.DataAnnotations;

namespace smarthomeAPI.Models
{
    [Index(nameof(EnvironmentID), nameof(LoggedTime),nameof(DeviceName))]
    public class RawData
    {
        public int Id { get; set; }
        public int EnvironmentID { get; set; }
        public int UserID { get; set; }
        public virtual EnvironmentType Environment { get; set; }
        public DateTime UploadTime { get; set; }
        public DateTime LoggedTime { get; set; }
        public string DeviceName { get; set; }
        public float accelerometer_x { get; set; }
        public float accelerometer_y { get; set; }
        public float accelerometer_z { get; set; }
        public float gyroscope_x { get; set; }
        public float gyroscope_y { get; set; }
        public float gyroscope_z { get; set; }
        public float compass_x { get; set; }
        public float compass_y { get; set; }
        public float compass_z { get; set; }

    }
}
