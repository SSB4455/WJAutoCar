# 使用机器学习让飞思卡尔小车实现自动驾驶（基于摄像头）

![Screenshot.png](https://github.com/SSB4455/WJAutoCar/blob/master/doc/Snipaste_2020-04-06_13-46-06.png)

## [ml-agents介绍](https://github.com/Unity-Technologies/ml-agents/blob/latest_release/docs/localized/zh-CN/docs/ML-Agents-Overview.md)(重点看)

- [ml-agents创建简单环境中文示例](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/localized/zh-CN/docs/Learning-Environment-Create-New.md)

## [安装ml-agents](https://github.com/Unity-Technologies/ml-agents/blob/788a34786094e974c1637f08a709640f72e1c755/docs/Installation.md)(了解)

- 安装unity 3018.4.18f1
- Install the com.unity.ml-agents Unity package
- [Unity ML-Agent之Agents设计](https://www.jianshu.com/p/6d40059a3454)(部分内容已经和最新版本不对应 但可以参考名词翻译部分)
- [码云同步项目地址](https://gitee.com/mirrors/Unity-ML-Agents)
- [深度学习PPO算法简介](https://zhuanlan.zhihu.com/p/38185553)

## [按照教程安装集成环境Anaconda](https://github.com/Unity-Technologies/ml-agents/blob/788a34786094e974c1637f08a709640f72e1c755/docs/Installation-Anaconda-Windows.md)

## 关于车轮组件问题

- 车身抖动是个综合现象
  - 不过有个罩住四个轮子的车体是必不可少的
- 车子漂移是要调整前向和侧向摩擦力
  - 需要结合悬挂和重力 很麻烦 而且还有可能导致车子又变抖

[WheelCollider](https://docs.unity3d.com/Manual/class-WheelCollider.html)示例里的摩擦力设置会导致车子抓地力无限大

![Inspector-WheelCollider2_demo.png](https://github.com/SSB4455/WJAutoCar/blob/master/doc/Inspector-WheelCollider2_demo.png)
![WheelFrictionCurve.png](https://github.com/SSB4455/WJAutoCar/blob/master/doc/WheelFrictionCurve.png)

[WheelCollider中文手册](http://docs.manew.com/Components/107.html)

## 完成游戏逻辑、小车控制、奖惩设置

- 调整ml-agents/config/trainer_config.yaml 里面的参数（不会调用默认参数就还行）
- 重命名为 mlagents-learn config/config.yaml 在命令行中运行：
  > mlagents-learn config/config.yaml --run-id=WJAutoCar-1 --train  
  > mlagents-learn config/config.yaml --env=E:/WJAutoCar/build/WJAutoCar.exe --run-id=WJAutoCar-1 --train
- (其中WJAutoCar是游戏中大脑的名字)
- 然后在unity中点击▶️运行
- 开始进行训练后，ml-agents 文件夹将 包含一个 summaries 目录。为了更详细地观测训练过程， 您可以使用 TensorBoard。在命令行中运行：
  > tensorboard --logdir=summaries
- 然后导航至 localhost:6006。
- [3D Balance Ball 环境入门](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/localized/zh-CN/docs/Getting-Started-with-Balance-Ball.md)
- [ml-agents创建简单环境中文示例](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/localized/zh-CN/docs/Learning-Environment-Create-New.md)
- [Unity3D ML-Agent-0.8.1 学习二（单代理学习）](https://blog.csdn.net/wangwei19871103/article/details/90345542)(写制作场景的过程可以参考)
- 从 TensorBoard 中，您将看到摘要统计信息：

  >- Lesson - 只有在进行 课程训练时才有意义。 3D Balance Ball 环境中不使用此项。
  >- Cumulative Reward - 所有 agent 的平均累积场景奖励。 在成功训练期间应该增大。
  >- Entropy - 模型决策的随机程度。在成功训练过程中 应该缓慢减小。如果减小得太快，应增大 beta 超参数。
  >- Episode Length - 所有 agent 在环境中每个场景的 平均长度。
  >- Learning Rate - 训练算法搜索最优 policy 时需要多大的 步骤。随着时间推移应该减小。
  >- Policy Loss - policy 功能更新的平均损失。与 policy （决定动作的过程）的变化程度相关。此项的幅度 在成功训练期间应该减小。
  >- Value Estimate - agent 访问的所有状态的平均价值估算。 在成功训练期间应该增大。
  >- Value Loss - 价值功能更新的平均损失。与模型 对每个状态的价值进行预测的能力相关。此项 在成功训练期间应该减小。

![WJAutoCar20200330训练效果图.png](https://github.com/SSB4455/WJAutoCar/blob/master/doc/WJAutoCar20200330%E8%AE%AD%E7%BB%83%E6%95%88%E6%9E%9C%E5%9B%BE.png)  

## 关于自动驾驶的思考

- 假设我们在训练一个外卖员送外卖 我们会给外卖员设定送到 朝向目标移动的奖励 同时我们会给外卖员不遵守交通规则以及超时的惩罚
  然而就在超时和遵守交通规则的两难里面 机器学习的外卖员自然也会像人类一样稍微不遵守交通规则从而在规定时间内送到 因为这样奖励更多
  所以在机器学习的训练中如何让模型不做那些人类伦理所不能接受的事情也是要注意的一个点

## 现存问题

- 训练时摄像头和多个位置速度传感器并存时训练得非常慢 甚至像没有使用摄像头摸黑训练一样
- 当车辆重置时车轮还有惯性旋转

## 已解决问题

- 车辆速度奖励获取异常  
  > 将奖励车辆速度的触发放在AgentAction方法中而不要放在Update中因为在训练过程中是加速的Update会比正常速度调用少很多
- 道路存在极少素材重叠
  > 使用ProGrids设置靠近吸附来拼接赛道就能够有效防止重叠和细小的缝隙
- 车辆控制非常差
  > 更换车轮组件[Wheel Controller 3D](https://assetstore.unity.com/packages/tools/physics/wheel-controller-3d-74512)

## 下一步要做

- PPO强化学习的中文解释
- Tensorflow训练参数解释
- 增加不同赛道
- 增加车辆及行人躲避训练
