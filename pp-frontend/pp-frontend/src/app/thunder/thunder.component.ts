import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ClientInfoDto } from '../models/dtos/ClientInfo';
import { ThunderService } from '../services/Thunder';

@Component({
  selector: 'app-thunder',
  templateUrl: './thunder.component.html',
  styleUrls: ['./thunder.component.scss'],
})
export class ThunderComponent {
    clientInfo?: ClientInfoDto;
    constructor(private thunderService: ThunderService,
        private snackBar: MatSnackBar) {

    }

    showSnackError(message?: string) {
        this.snackBar.open(`❌ ${message ?? "Error while executing the request"}`, 'Dismiss', {
        duration: 5000
        });
    }

    getStats(button: {disabled: boolean}) {
        button.disabled = true;
        this.thunderService.getClientData().subscribe(res => {
            try {
                if (res.success) {
                    this.clientInfo = res.body;
                } else {
                    this.showSnackError(res.error);
                }
            } finally {
                button.disabled = false;
            }
        })
    }

    sort(button: {disabled: boolean}) {
        button.disabled = true;
        this.thunderService.sort().subscribe(res => {
            try {
                if (!res.success) {
                    this.showSnackError(res.error);
                }
            } finally {
                button.disabled = false;
            }
        })
    }

}