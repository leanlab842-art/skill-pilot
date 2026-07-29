import { Link } from "react-router-dom";
import { ArrowRight } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { CircularGauge } from "@/components/common/CircularGauge";
import { getMatchRateStrokeClass, getMatchRateTextClass } from "@/lib/matchRate";
import { cn } from "@/lib/utils";

interface MatchRateHeroProps {
  matchRate: number;
  analysisId: string;
}

/**
 * 分析結果画面で最も目立たせたい要素。マッチ率の円形ゲージと、学習ロードマップへの
 * プライマリー導線を1つのカードにまとめ、スクロールせず視界に入るようにする。
 */
export function MatchRateHero({ matchRate, analysisId }: MatchRateHeroProps) {
  return (
    <Card>
      <CardContent className="flex flex-col items-center gap-6 py-10 text-center">
        <p className="text-sm font-medium text-muted-foreground">必須スキルの一致率</p>

        <CircularGauge
          value={matchRate}
          label="必須スキルの一致率"
          size={176}
          strokeWidth={14}
          colorClassName={getMatchRateStrokeClass(matchRate)}
        >
          <span className={cn("text-4xl font-bold tabular-nums", getMatchRateTextClass(matchRate))}>
            {matchRate}%
          </span>
        </CircularGauge>

        <Button size="lg" nativeButton={false} render={<Link to={`/analyses/${analysisId}/roadmap`} />}>
          学習ロードマップを見る
          <ArrowRight />
        </Button>
      </CardContent>
    </Card>
  );
}
