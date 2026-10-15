<!-- markdownlint-disable-file -->
# Plan critique: memento

Keep the caretaker responsible only for retaining and replacing the prior opaque snapshot. `Cart` should own snapshot capture, restore and line copying so adding a cart field cannot silently break undo. Preserve the current behavior that only successful item changes replace the saved one-level undo state.
