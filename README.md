# 使用机器学习让飞思卡尔小车实现自动驾驶（基于摄像头）

## [ml-agents介绍](https://github.com/Unity-Technologies/ml-agents/blob/latest_release/docs/localized/zh-CN/docs/ML-Agents-Overview.md)(重点看)

- [ml-agents创建简单环境中文示例](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/localized/zh-CN/docs/Learning-Environment-Create-New.md)

## [安装ml-agents](https://github.com/Unity-Technologies/ml-agents/blob/788a34786094e974c1637f08a709640f72e1c755/docs/Installation.md)(了解)

- 安装unity 3018.4.18f1
- Install the com.unity.ml-agents Unity package

## [按照教程安装集成环境Anaconda](https://github.com/Unity-Technologies/ml-agents/blob/788a34786094e974c1637f08a709640f72e1c755/docs/Installation-Anaconda-Windows.md)

## 关于车轮组件问题

车身抖动是个综合现象
 不过有个罩住四个轮子的车体是必不可少的

车子漂移是要调整前向和侧向摩擦力
 需要结合悬挂和重力 很麻烦 而且还有可能导致车子又变抖

[WheelCollider](https://docs.unity3d.com/Manual/class-WheelCollider.html)示例里的摩擦力设置会导致车子抓地力无限大

![Inspector-WheelCollider2_demo.png](https://github.com/SSB4455/WJAutoCar/blob/master/doc/Inspector-WheelCollider2_demo.png)
![WheelFrictionCurve.png](https://github.com/SSB4455/WJAutoCar/blob/master/doc/WheelFrictionCurve.png)

[WheelCollider中文手册](http://docs.manew.com/Components/107.html)

## 完成游戏逻辑、小车控制、奖惩设置

- 调整ml-agents/config/trainer_config.yaml 里面的参数（不调也行 就是可能训练效率会比较差）
- 重命名为 mlagents-learn config/config.yaml
- 运行 mlagents-learn config/config.yaml --run-id=WJAutoCar-1 --train
  - 其中WJAutoCar是游戏中大脑的名字
- 然后在unity中点击运行
- 开始进行训练后，ml-agents 文件夹将 包含一个 summaries 目录。为了更详细地观测训练过程， 您可以使用 TensorBoard。从命令行中运行：
- 运行 tensorboard --logdir=summaries
- 然后导航至 localhost:6006。
- [3D Balance Ball 环境入门](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/localized/zh-CN/docs/Getting-Started-with-Balance-Ball.md)
- [ml-agents创建简单环境中文示例](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/localized/zh-CN/docs/Learning-Environment-Create-New.md)
- 从 TensorBoard 中，您将看到摘要统计信息：
>
>- Lesson - 只有在进行 课程训练时才有意义。 3D Balance Ball 环境中不使用此项。
>- Cumulative Reward - 所有 agent 的平均累积场景奖励。 在成功训练期间应该增大。
>- Entropy - 模型决策的随机程度。在成功训练过程中 应该缓慢减小。如果减小得太快，应增大 beta 超参数。
>- Episode Length - 所有 agent 在环境中每个场景的 平均长度。
>- Learning Rate - 训练算法搜索最优 policy 时需要多大的 步骤。随着时间推移应该减小。
>- Policy Loss - policy 功能更新的平均损失。与 policy （决定动作的过程）的变化程度相关。此项的幅度 在成功训练期间应该减小。
>- Value Estimate - agent 访问的所有状态的平均价值估算。 在成功训练期间应该增大。
>- Value Loss - 价值功能更新的平均损失。与模型 对每个状态的价值进行预测的能力相关。此项 在成功训练期间应该减小。

## 现存问题

- 车辆控制还不像模型车（例如高速过弯会翻车）
- 当车辆重置时车轮还有惯性旋转
- 道路存在极少素材重叠

## 下一步要做

- 训练次数与时间和显示时间的对比
- PPO强化学习的中文解释
- 做一个测试平台
  - 包含一段直路
    - 记录到终点时的速度
  - 自动向前加速
  - 弹出框实时调节车轮及车身参数（质量 悬挂 摩擦力等）
  - 一段90度的弯
    - 测试漂移情况和翻车
