<!-- markdownlint-disable-file -->
# Plan critique: mediator

The mediator should own the widget coordination graph and refresh order. Avoid merely replacing widget peer fields with a mediator while leaving `StoreConsole` coupled to peer properties; route its widget actions through the mediator too. Preserve which refresh sequence each user action triggers and leave non-S17 cart logic unchanged.
