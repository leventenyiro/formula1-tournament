import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Statistics } from 'app/models/statistics';
import { SeasonService } from 'app/services/season.service';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.scss']
})
export class StatisticsComponent implements OnInit {
  inputSearch = new FormControl('');
  statistics?: Statistics;
  driverName?: string;
  isFetching?: boolean = false;
  noData?: boolean = false;

  constructor(
    private seasonService: SeasonService, 
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    if (this.route.snapshot.queryParamMap.get('name')) {
      this.inputSearch.setValue(this.route.snapshot.queryParamMap.get('name'));
      this.onSearch();
    }
  }

  onSearch(): void {
    this.noData = false;
    this.isFetching = true;
    this.driverName = '';
    this.statistics = undefined;
    window.history.pushState("Statistics", "Car Racing Tournament", `statistics?name=${this.inputSearch.value}`);

    this.seasonService.getStatistics(this.inputSearch.value).subscribe({
      next: data => {
        this.statistics = data;
        this.driverName = this.inputSearch.value;
        this.isFetching = false;
      },
      error: () => {
        this.noData = true;
        this.isFetching = false;
      }
    });
  }

  calculatePercentage(part: number, full: number): string {
    return (part / full * 100).toFixed(1);
  }

  seasonMax(): number {
    return this.statistics?.seasonStatistics?.reduce((max, stat) => Math.max(max, stat.position ?? 0), 0) ?? 0;        
  }

  positionMax(): number {
    return this.statistics?.positionStatistics?.reduce((max, stat) => Math.max(max, stat.number ?? 0), 0) ?? 0;        
  }
}