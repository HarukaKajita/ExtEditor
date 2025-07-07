@echo off

rem ---------- Rem/||()でコメントアウトしてる。中身が実行されていない。----------
rem ---------- Rem/||(と)自体をコメントアウトすると、Unityから実行しても独立したコンソールで実行できるので、終了時に閉じられなくなる。----------
rem ---------- 出力結果を読めるようになるのでデバッグ目的に使用する。----------
Rem/||(
    rem ---------- launch self in new console once ---
    if "%~1"=="" (
        start "" cmd /k "%~f0" _inner
        goto :eof
    )
)
rem ---------- normal path below (arg = _inner) ----------


REM Convert all Markdown files in the same directory as this script to PDF using md-to-pdf
REM Requires: md-to-pdf (https://www.npmjs.com/package/md-to-pdf) installed and available in PATH

SETLOCAL ENABLEDELAYEDEXPANSION

REM Change to the directory of this script
pushd "%~dp0"

echo ================================================
echo Converting all *.md files in %CD% to PDF …
echo ================================================

for %%F in (*.md *.MD) do (
    echo [INFO] Processing "%%F"
    md-to-pdf "%CD%\%%F"
    if ERRORLEVEL 1 (
        echo   [ERROR] Failed to convert "%%F"
    ) else (
        echo   [OK] "%%~nF.pdf" created
    )
)

echo.
echo All conversions finished.

popd

ENDLOCAL

REM Pause so the user can read the output if double-clicked
rem echo.
rem echo Script finished. Press any key to close this window.
rem pause