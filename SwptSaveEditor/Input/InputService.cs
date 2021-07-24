// Copyright 2021 Crystal Ferrai
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using SwptSaveEditor.Document;
using SwptSaveEditor.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SwptSaveEditor.Input
{
    /// <summary>
    /// Service for registering and handling input actions
    /// </summary>
    internal class InputService
    {
        private readonly DocumentService mDocumentService;

        private readonly List<InputAction> mGlobalActions;

        /// <summary>
        /// Globally available input actions. These are checked after document specific actions
        /// </summary>
        public IList<InputAction> GlobalActions => mGlobalActions;

        /// <summary>
        /// If true, will suppress all input handling by this service
        /// </summary>
        public bool SuppressActions { get; set; }

        public InputService(IServiceProvider services)
        {
            services.Inject(out mDocumentService);

            mGlobalActions = new List<InputAction>();
        }

        /// <summary>
        /// Processes a PreviewKeyDown event and handles it if there is a matching action in the current
        /// document or the global actions list
        /// </summary>
        public void ProcessPreviewKeyDown(KeyEventArgs args)
        {
            if (SuppressActions) return;
            if (args.Handled) return;

            IDocument doc = mDocumentService.GetActiveDocument();
            if (doc != null)
            {
                ProcessActions(doc.InputActions, args);
            }

            if (args.Handled) return;

            ProcessActions(mGlobalActions, args);
        }

        public static void ProcessActions(IEnumerable<InputAction> actions, InputEventArgs args)
        {
            foreach (InputAction action in actions)
            {
                if (action.Matches(null, args))
                {
                    if (action.Command.CanExecute(null))
                    {
                        action.Command.Execute(null);
                    }
                    args.Handled = true;
                    return;
                }
            }
        }
    }
}
