import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RunnerStateDto } from '../models/dtos/RunnerState';
import { RunnerService } from '../services/Runner';


@Component({
  selector: 'app-firefly-iii-pp',
  templateUrl: './firefly-iii-pp.component.html',
  styleUrls: ['./firefly-iii-pp.component.scss'],
})
export class FireflyIIIPPComponent {
  status: RunnerStateDto | undefined;
  busy: boolean = false;
  timer: NodeJS.Timer | undefined;
  constructor(private runnerService: RunnerService,
        private snackBar: MatSnackBar) {
    this.refreshStatus();
  }

  refreshStatus() {
    this._refreshStatus();
    this.startRefreshTimer();
  }

  stopJob() {
    if (this.busy){
      return;
    }
    this.busy = true;
    this.runnerService.stopJob().subscribe(res => {
      try {
        if (res.success) {
          this.status = res.body;
        } else {
          this.status = undefined;
          this.showSnackError(res.error);
        }
      } finally {
        this.busy = false;
      }
    });
  }

  _refreshStatus() {
    if (this.busy){
      return;
    }
    this.busy = true;
    this.runnerService.getStatus().subscribe(res => {
      try {
        if (res.success) {
          this.status = res.body;
        } else {
          this.status = undefined;
          this.showSnackError(res.error);
        }
      } finally {
        this.busy = false;
      }
    });
  }

  startRefreshTimer() {
    clearInterval(this.timer);
    this.timer = setInterval(() => {
      if (this.status && this.status.state === "running") {
        this._refreshStatus();
      } else {
        clearInterval(this.timer);
      }
    }, 1000);
  }

  getProgress() {
    if (this.status
      && this.status.state === "running"
      && this.status.totalTransactions > 0
      && this.status.completedTransactions <= this.status.totalTransactions) {
        return (this.status.completedTransactions / this.status.totalTransactions) * 100;
    }
    return 0;
  }

  showSnackError(error?: string) {
    this.snackBar.open(error ?? "Error while executing the request", 'Dismiss', {
      duration: 5000
    });
  }
}