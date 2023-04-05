#!/usr/bin/env sh

APP_NAME="ToneAudioPlayer"
APP_ASSETS_PATH="${APP_NAME}/Assets/"
APP_ICON_SVG="${APP_ASSETS_PATH}Images/Icons/Home/music.svg"
APP_ICON_PATH="${APP_ASSETS_PATH}icon.ico"
ICO_SIZES="16 32 48 64 96 128 192 256"
ANDROID_ICON_SIZE="400"
ANDROID_ICON_PATH="$APP_NAME.Android/Icon.png"
TMP_PATH="/tmp/${APP_NAME}/"



rm -rf "$TMP_PATH"
mkdir "$TMP_PATH"

for i in $ICO_SIZES; do
   #convert "${APP_ICON_SVG}" -scale $i $i.png   
   # inkscape quality is way better than convert
   inkscape -w $i -h $i "${APP_ICON_SVG}" -o "${TMP_PATH}$i.png"
done

convert "${TMP_PATH}*.png" "${APP_ICON_PATH}"

inkscape -w "$ANDROID_ICON_SIZE" -h "$ANDROID_ICON_SIZE" "${APP_ICON_SVG}" -o "$ANDROID_ICON_PATH"
