﻿//------------------------------------------------------------------------------
// <copyright file="UltraTimeSpanEditorHelper.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    class UltraTimeSpanEditorHelper
    {

        public TimeSpan MinValue { get; set; }
        public TimeSpan MaxValue { get; set; }
        public TimeSpan DefaultValue { get; set; }

         public UltraTimeSpanEditorHelper(TimeSpan minValue, TimeSpan maxValue, TimeSpan defaultValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            DefaultValue = defaultValue;
        }

        public UltraTimeSpanEditorHelper(int minValue, int maxValue, int defaultValue):
          this(TimeSpan.FromSeconds(minValue), TimeSpan.FromSeconds(maxValue), TimeSpan.FromSeconds(defaultValue))
            {
            }

        /// <summary>
        /// Set max and min, add buttons, and attach events for a standard "oven timer" timespan editor
        /// </summary>
        /// <param name="editor"></param>
        public void SetDefaults(UltraTimeSpanEditor editor)
        {
            editor.MaxValue = MaxValue;
            editor.MinValue = MinValue;
            SetEventsToControl(editor);
        }

        /// <summary>
        /// Attach events for a standard timespan editor
        /// </summary>
        /// <param name="editor"></param>
        public void SetEventsToControl(UltraTimeSpanEditor editor)
        {
            editor.SpinButtonVisible = false;
            editor.ButtonsRight.Add(new SpinEditorButton());
            editor.EditorSpinButtonClick += CreateEditorSpinButtonClickHandler();
            editor.ValidationError += CreateTimeSpanEditorValidationErrorHandler();
        }

        /// <summary>
        /// Create a control container for use in an UltraWinGrid
        /// Example:
        /// helper = new UltraTimeSpanEditorHelper(Constants.MinServerPingIntervalSeconds,
        ///                                        Constants.MaxServerPingIntervalSeconds,
        ///                                        Constants.DefaultServerPingIntervalSeconds);
        /// column.EditorComponent = helper.CreateControlContainer(Controls);
        /// </summary>
        /// <param name="controls">The control collection for the vie</param>
        public UltraControlContainerEditor CreateControlContainer(Control.ControlCollection controls)
        {
            var container = new UltraControlContainerEditor();
            var editor = new UltraTimeSpanEditor();
            var renderer = new UltraTimeSpanEditor();

            SetEventsToControl(editor);
            SetEventsToControl(renderer);

            editor.BorderStyle = UIElementBorderStyle.None;
            renderer.BorderStyle = UIElementBorderStyle.None;

            editor.Appearance.BorderColor = Color.White;
            renderer.Appearance.BorderColor = Color.White;

            controls.Add(editor);
            controls.Add(renderer);

            container.EditingControl = editor;
            container.RenderingControl = renderer;

            container.ApplyOwnerAppearanceToEditingControl = true;
            container.ApplyOwnerAppearanceToRenderingControl = true;

            container.EditingControlPropertyName = "Value";
            container.RenderingControlPropertyName = "Value";

            container.EnterEditModeMouseBehavior = EnterEditModeMouseBehavior.EnterEditModeAndClick;
            
            return container;
        }


        public TimeSpanEditorValidationErrorHandler CreateTimeSpanEditorValidationErrorHandler()
        {
            return CreateTimeSpanEditorValidationErrorHandler(MinValue, MaxValue, DefaultValue);
        }

        public static TimeSpanEditorValidationErrorHandler CreateTimeSpanEditorValidationErrorHandler(TimeSpan minValue, TimeSpan maxValue, TimeSpan defaultValue)
        {
            return delegate(object sender, TimeSpanEditorValidationErrorEventArgs e)
                       {
                           e.PromptUser = false;
                           e.RetainFocus = false; 
                           ResetInvalidData(e, (UltraTimeSpanEditor)sender, minValue, maxValue, defaultValue);
                       };
        }

        public SpinButtonClickEventHandler CreateEditorSpinButtonClickHandler()
        {
            return CreateEditorSpinButtonClickHandler(MinValue, MaxValue, DefaultValue);
        }

        public static SpinButtonClickEventHandler CreateEditorSpinButtonClickHandler(TimeSpan minValue, TimeSpan maxValue, TimeSpan defaultValue)
        {

            return delegate(object sender, SpinButtonClickEventArgs e)
                       {
                           SpinButtonOnClick(sender, e, minValue, maxValue, defaultValue);
                       };
        }

        /// <summary>
        /// Reset invalid data to the nearest max, min, or to the default value
        /// </summary>
        public static void ResetInvalidData(TimeSpanEditorValidationErrorEventArgs e, UltraTimeSpanEditor editor, TimeSpan minValue, TimeSpan maxValue, TimeSpan defaultValue)
        {
            if (e == null)
            {
                editor.Value = defaultValue;
            }
            else if (e.Reason == TimeSpanEditorInvalidValueReason.GreaterThanMax)
            {
                editor.Value = maxValue;
            }
            else
            {
                if (e.Reason == TimeSpanEditorInvalidValueReason.LessThanMin)
                {
                    editor.Value = minValue;
                }
                else
                {
                    editor.Value = defaultValue;
                }
            }
        }

        /// <summary>
        /// Create "oven timer" functionality for spinner button clicks
        /// This will increment or decrement by varying amounts based on the current value
        /// </summary>
        public static void SpinButtonOnClick(object sender, SpinButtonClickEventArgs e,TimeSpan minValue, TimeSpan maxValue, TimeSpan defaultValue)
        {
            if (!e.Button.Editor.IsValid)
            {
                ResetInvalidData(null, (UltraTimeSpanEditor)sender, minValue, maxValue, defaultValue);
                return;
            }
            if (e.Button.Editor.Value == null)
            {
                e.Button.Editor.Value =
                    TimeSpan.FromSeconds(Properties.Constants.DefaultScheduledRefreshIntervalSeconds);
            }
            else
            {
                TimeSpan currentVal = ((UltraTimeSpanEditor)e.Context).TimeSpan;

                if (e.ButtonType == SpinButtonItem.NextItem)
                {
                    ((UltraTimeSpanEditor)e.Context).TimeSpan = AddTime(currentVal,
                                                                         maxValue);
                }
                else
                {
                    ((UltraTimeSpanEditor)e.Context).TimeSpan = SubtractTime(currentVal,
                                                                              minValue);
                }
            }
        }

        public static TimeSpan SubtractTime(TimeSpan t, TimeSpan minValue)
        {
            if (t.TotalSeconds <= minValue.TotalSeconds)
                return minValue;

            TimeSpan tempTime = minValue;
            if (t.TotalHours >= 1)
            {
                tempTime = t.Subtract(TimeSpan.FromMinutes(30));
            }
            else if (t.TotalMinutes > 1)
            {
                tempTime = t.Subtract(TimeSpan.FromMinutes(1));
            }
            else
            {
                if (t.TotalSeconds > 10)
                {
                    tempTime = t.Subtract(TimeSpan.FromSeconds(10));

                }
                else if (t.TotalSeconds > 1)
                {
                    tempTime = t.Subtract(TimeSpan.FromSeconds(1));
                }
            }

            if (tempTime.TotalSeconds <= minValue.TotalSeconds)
                return minValue;
            return tempTime;
        }

        public static TimeSpan AddTime(TimeSpan t, TimeSpan maxValue)
        {
            if (t.TotalSeconds >= maxValue.TotalSeconds)
                return maxValue;

            TimeSpan tempTime = maxValue;
            if (t.TotalSeconds < 10)
            {
                tempTime = t.Add(TimeSpan.FromSeconds(1));
            }
            else
            {
                if (t.TotalSeconds < 60)
                {
                    tempTime = t.Add(TimeSpan.FromSeconds(10));
                }
                else if (t.TotalHours >= 1)
                {
                    tempTime = t.Add(TimeSpan.FromMinutes(30));
                }
                else if (t.TotalMinutes >= 1)
                {
                    tempTime = t.Add(TimeSpan.FromMinutes(1));
                }
            }
            if (tempTime.TotalSeconds >= maxValue.TotalSeconds)
                return maxValue;
            return tempTime;
        }

    }
}
