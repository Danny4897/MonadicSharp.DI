#!/bin/bash
# commit-commands: MCP wrapper for conventional commits

COMMIT_MSG="$1"

# Convenzioni commit semantiche
if [[ -z "$COMMIT_MSG" ]]; then
    echo "Usage: commit-commands <message>"
    echo ""
    echo "Conventional commit types:"
    echo "  feat:     A new feature"
    echo "  fix:      A bug fix"
    echo "  docs:     Documentation only changes"
    echo "  style:    Changes that don't affect code meaning"
    echo "  refactor: Code change that neither fixes a bug nor adds a feature"
    echo "  perf:     Performance improvements"
    echo "  test:     Adding or correcting tests"
    echo "  chore:    Changes to build process or auxiliary tools"
    exit 1
fi

# Suggerimento formato commit convenzionale
echo "Suggested format:"
echo "  <type>[(scope)]: <description>"
echo ""
echo "Examples:"
echo "  feat: add Result<T> monad"
echo "  fix(di): resolve handler registration bug"
echo "  docs: update README with examples"
echo "  test: add integration tests for mediator"
