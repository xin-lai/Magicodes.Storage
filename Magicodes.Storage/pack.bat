
echo 包搜索字符串
echo %1
echo 项目方案地址
echo %2

echo 打包
nuget Pack %2 -Build -Properties Configuration=Release