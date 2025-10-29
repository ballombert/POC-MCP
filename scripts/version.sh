#!/bin/bash

# Version Management Script for POC-MCP
# Usage: ./scripts/version.sh [action] [args...]

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"
PROPS_FILE="$ROOT_DIR/Directory.Build.props"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Extract current version from Directory.Build.props
get_current_version() {
    local major=$(grep '<VersionMajor>' "$PROPS_FILE" | sed 's/.*<VersionMajor>\(.*\)<\/VersionMajor>.*/\1/')
    local minor=$(grep '<VersionMinor>' "$PROPS_FILE" | sed 's/.*<VersionMinor>\(.*\)<\/VersionMinor>.*/\1/')
    local patch=$(grep '<VersionPatch>' "$PROPS_FILE" | sed 's/.*<VersionPatch>\(.*\)<\/VersionPatch>.*/\1/')
    local suffix=$(grep '<VersionSuffix>' "$PROPS_FILE" | sed 's/.*<VersionSuffix>\(.*\)<\/VersionSuffix>.*/\1/' | head -n1)
    
    if [ -n "$suffix" ]; then
        echo "$major.$minor.$patch-$suffix"
    else
        echo "$major.$minor.$patch"
    fi
}

# Get individual version components
get_version_component() {
    case $1 in
        major) grep '<VersionMajor>' "$PROPS_FILE" | sed 's/.*<VersionMajor>\(.*\)<\/VersionMajor>.*/\1/' ;;
        minor) grep '<VersionMinor>' "$PROPS_FILE" | sed 's/.*<VersionMinor>\(.*\)<\/VersionMinor>.*/\1/' ;;
        patch) grep '<VersionPatch>' "$PROPS_FILE" | sed 's/.*<VersionPatch>\(.*\)<\/VersionPatch>.*/\1/' ;;
        suffix) grep '<VersionSuffix>' "$PROPS_FILE" | sed 's/.*<VersionSuffix>\(.*\)<\/VersionSuffix>.*/\1/' | head -n1 ;;
    esac
}

# Update version component in Directory.Build.props
update_version_component() {
    local component=$1
    local value=$2
    local tag
    
    case $component in
        major) tag="VersionMajor" ;;
        minor) tag="VersionMinor" ;;
        patch) tag="VersionPatch" ;;
        suffix) tag="VersionSuffix" ;;
    esac
    
    if [ "$component" = "suffix" ]; then
        if [ -z "$value" ]; then
            # Remove suffix line if empty
            sed -i '' "/<VersionSuffix>/d" "$PROPS_FILE"
        else
            # Update or add suffix
            if grep -q "<VersionSuffix>" "$PROPS_FILE"; then
                sed -i '' "s|<VersionSuffix>.*</VersionSuffix>|<VersionSuffix>$value</VersionSuffix>|" "$PROPS_FILE"
            else
                sed -i '' "/<VersionPatch>/a\\
    <VersionSuffix>$value</VersionSuffix>" "$PROPS_FILE"
            fi
        fi
    else
        sed -i '' "s|<$tag>.*</$tag>|<$tag>$value</$tag>|" "$PROPS_FILE"
    fi
}

# Show current version
show_version() {
    local current=$(get_current_version)
    log_info "Current version: $current"
    
    local major=$(get_version_component major)
    local minor=$(get_version_component minor)
    local patch=$(get_version_component patch)
    local suffix=$(get_version_component suffix)
    
    echo "  Major: $major"
    echo "  Minor: $minor"
    echo "  Patch: $patch"
    echo "  Suffix: ${suffix:-"(none)"}"
}

# Bump version
bump_version() {
    local component=$1
    local current_major=$(get_version_component major)
    local current_minor=$(get_version_component minor)
    local current_patch=$(get_version_component patch)
    local current_suffix=$(get_version_component suffix)
    
    log_info "Current version: $(get_current_version)"
    
    case $component in
        major)
            update_version_component major $((current_major + 1))
            update_version_component minor 0
            update_version_component patch 0
            ;;
        minor)
            update_version_component minor $((current_minor + 1))
            update_version_component patch 0
            ;;
        patch)
            update_version_component patch $((current_patch + 1))
            ;;
        *)
            log_error "Invalid component: $component. Use major, minor, or patch."
            exit 1
            ;;
    esac
    
    local new_version=$(get_current_version)
    log_success "Version bumped to: $new_version"
}

# Set version suffix
set_suffix() {
    local suffix=$1
    update_version_component suffix "$suffix"
    log_success "Version suffix set to: ${suffix:-"(removed)"}"
    log_info "New version: $(get_current_version)"
}

# Release version (remove suffix)
release_version() {
    local current_suffix=$(get_version_component suffix)
    if [ -z "$current_suffix" ]; then
        log_warning "Version is already a release version (no suffix)"
        return
    fi
    
    update_version_component suffix ""
    log_success "Released version: $(get_current_version)"
}

# Set specific version
set_version() {
    local version=$1
    
    # Parse version string (e.g., "1.2.3-beta" or "1.2.3")
    if [[ $version =~ ^([0-9]+)\.([0-9]+)\.([0-9]+)(-(.+))?$ ]]; then
        local major=${BASH_REMATCH[1]}
        local minor=${BASH_REMATCH[2]}
        local patch=${BASH_REMATCH[3]}
        local suffix=${BASH_REMATCH[5]:-""}
        
        update_version_component major "$major"
        update_version_component minor "$minor"
        update_version_component patch "$patch"
        update_version_component suffix "$suffix"
        
        log_success "Version set to: $(get_current_version)"
    else
        log_error "Invalid version format: $version"
        log_error "Expected format: major.minor.patch[-suffix]"
        exit 1
    fi
}

# Build and pack
build_and_pack() {
    log_info "Building and packing with version: $(get_current_version)"
    
    cd "$ROOT_DIR"
    dotnet clean
    dotnet build --configuration Release
    dotnet pack --configuration Release --no-build
    
    log_success "Build and pack completed"
    log_info "Package location: src/SampleMcpServer/bin/Release/SampleMcpServer.$(get_current_version).nupkg"
}

# Show usage
show_usage() {
    echo "Version Management Script for POC-MCP"
    echo ""
    echo "Usage: $0 [command] [args...]"
    echo ""
    echo "Commands:"
    echo "  show                    Show current version"
    echo "  bump <major|minor|patch> Bump version component"
    echo "  set <version>           Set specific version (e.g., 1.2.3-beta)"
    echo "  suffix <suffix>         Set version suffix (e.g., beta, rc1)"
    echo "  release                 Remove version suffix (release version)"
    echo "  pack                    Build and pack NuGet package"
    echo "  help                    Show this help"
    echo ""
    echo "Examples:"
    echo "  $0 show"
    echo "  $0 bump patch"
    echo "  $0 set 1.0.0-rc1"
    echo "  $0 suffix beta"
    echo "  $0 release"
    echo "  $0 pack"
}

# Main script logic
case ${1:-""} in
    show)
        show_version
        ;;
    bump)
        if [ -z "$2" ]; then
            log_error "Bump requires a component (major, minor, patch)"
            exit 1
        fi
        bump_version "$2"
        ;;
    set)
        if [ -z "$2" ]; then
            log_error "Set requires a version string"
            exit 1
        fi
        set_version "$2"
        ;;
    suffix)
        set_suffix "$2"
        ;;
    release)
        release_version
        ;;
    pack)
        build_and_pack
        ;;
    help|--help|-h)
        show_usage
        ;;
    "")
        show_usage
        ;;
    *)
        log_error "Unknown command: $1"
        show_usage
        exit 1
        ;;
esac