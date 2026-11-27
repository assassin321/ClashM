@echo off
chcp 65001 >nul
title 卸载 ClashM Elevated Service

:: 检查管理员权限
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo 请以管理员身份运行此脚本
    pause
    exit /b 1
)

set SERVICE_NAME=ClashMElevated

:: 停止服务
echo 正在停止服务...
sc stop %SERVICE_NAME%
timeout /t 2 /nobreak >nul

:: 删除服务
echo 正在删除服务...
sc delete %SERVICE_NAME%

:: 可选：删除注册表配置
echo 是否删除注册表配置？（y/n）
set /p DELETE_REG=
if /i "%DELETE_REG%"=="y" (
    reg delete HKLM\SOFTWARE\ClashM /f
    echo 注册表配置已删除
)

echo 服务已卸载
pause