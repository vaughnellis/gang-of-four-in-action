<!-- markdownlint-disable-file -->
# Plan critique: singleton

Keep both singleton instances safe for concurrent use: `Lazy<T>` protects initialization, configuration accessors synchronize state, and log writer changes and writes share a lock. Preserve scenario reset and output restoration. Avoid adding a DI package; explain how a host can register the existing singleton instances.
